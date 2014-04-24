using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsGame1.Managers;
using DigitalRune.Game;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;
using Microsoft.Xna.Framework.GamerServices;

namespace WindowsGame1.Windows
{
    public class CreateUser : Window
    {
        private EmoEngineManager emoEngine;
        private TextBox nameBox;
        public CreateUser(EmoEngineManager emoEngine)
        {
            this.emoEngine = emoEngine;
            IsModal = true;
            Title = "New User";
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
                Orientation = Orientation.Horizontal,
                Margin = new Vector4F(5),
            };
            Content = stackPanel;
            

            nameBox = new TextBox
            {
                Name = "NewName",
                Text = "New User",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Vector4F(3),
                MaxWidth = 250,
            };
            stackPanel.Children.Add(nameBox);

            Button createUser = new Button
            {
                Name = "CreateUser",
                Content = new TextBlock { Text = "Create User" },
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Vector4F(3),
            };
            createUser.Click += CreateUserOnClick;
            stackPanel.Children.Add(createUser);
        }

        private void CreateUserOnClick(object sender, EventArgs eventArgs)
        {
            String profileName = nameBox.Text;
            if (!profileName.Equals("New User"))
            {
                emoEngine.SaveProfile(profileName);
                emoEngine.LoadProfile(profileName);
                Dispose();
            }
            else
            {
                //show message box here
            }
        }

        private void Dispose()
        {
            Close();
        }
    }
}
