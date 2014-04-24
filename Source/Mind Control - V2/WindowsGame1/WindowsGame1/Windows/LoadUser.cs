using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using WindowsGame1.Managers;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;

namespace WindowsGame1.Windows
{
    public class LoadUser : Window
    {
        private EmoEngineManager emoEngine;
        private DropDownButton profileList;
        public LoadUser(EmoEngineManager emoEngine)
        {
            this.emoEngine = emoEngine;
            IsModal = true;
            Title = "Load User";
            HideOnClose = false;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;
            Initialize();
        }

        private void Initialize()
        {
            StackPanel stackPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Vertical,
            };
            Content = stackPanel;
            
            //create the visuals to load the screen
            var loadPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Horizontal,
                Margin = new Vector4F(5),
            };
            stackPanel.Children.Add(loadPanel);

            string[] profileNames = emoEngine.GetProfileNames();
            profileList = new DropDownButton
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Vector4F(3),
                MaxDropDownHeight = 250,
            };
            loadPanel.Children.Add(profileList);

            foreach (string name in profileNames)
            {
                profileList.Items.Add(name);
            }
            profileList.SelectedIndex = 0;

            var loadButton = new Button
            {
                Name = "LoadUser",
                Content = new TextBlock {Text = "Load User"},
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Vector4F(3),
            };
            loadButton.Click += LoadButtonOnClick;
            loadPanel.Children.Add(loadButton);

            //create the visual for the option to create new user
            var createPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Horizontal,
                Margin = new Vector4F(5),
            };
            stackPanel.Children.Add(createPanel);

            var createButton = new Button
            {
                Name = "CreateUser",
                Content = new TextBlock{Text = "New User"},
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Vector4F(3),
            };
            createButton.Click += CreateButtonOnClick;
            createPanel.Children.Add(createButton);
        }

        private void CreateButtonOnClick(object sender, EventArgs eventArgs)
        {
            Window createUser = new CreateUser(emoEngine);
            Screen.Children.Add(new CreateUser(emoEngine));
            Dispose();
        }

        private void LoadButtonOnClick(object sender, EventArgs eventArgs)
        {
            string profileName = (string)profileList.Items[profileList.SelectedIndex];
            emoEngine.LoadProfile(profileName);
            Dispose();
        }

        private void Dispose()
        {
            Close();
        }
    }
}
