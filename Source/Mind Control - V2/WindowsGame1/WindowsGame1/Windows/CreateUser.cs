using System;
using System.Linq;
using System.Text.RegularExpressions;
using WindowsGame1.Managers;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;

namespace WindowsGame1.Windows
{
    public class CreateUser : Window
    {
        private bool clearText = false;
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
            CloseButtonStyle = "";
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
            nameBox.PropertyChanged += (s, e) =>
            {
                if (e.Property.Name.Equals("IsFocused") && nameBox.Text.Equals("New User"))
                    clearText = true;
            };
            stackPanel.Children.Add(nameBox);

            Button createUser = new Button
            {
                Name = "CreateUser",
                Content = new TextBlock { Text = "Create User" },
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Vector4F(3),
                Focusable = false,
                FocusWhenMouseOver = false,
            };
            createUser.Click += CreateUserOnClick;
            stackPanel.Children.Add(createUser);
        }

        private void CreateUserOnClick(object sender, EventArgs eventArgs)
        {
            String profileName = nameBox.Text;
            string[] profileNames = emoEngine.GetProfileNames();
            if (profileName.Equals("New User") || profileName.Equals(""))
            {
                var messageBox = new MessageBox("You must give a profile name.");
                messageBox.Show(Screen);
            }
            else
            {
                if (!profileNames.Contains(profileName))
                {
                    Match match = Regex.Match(profileName, @"^([\w ]*)$");
                    if (match.Success)
                    {
                        emoEngine.CreateProfile(profileName);
                        emoEngine.LoadProfile(profileName);
                        Close();
                    }
                    else
                    {
                        //message box
                        //invalid characters
                        var messageBox = new MessageBox("Invalid Characters", "Special characters are not allowed.");
                        messageBox.Show(Screen);
                    }
                }
                else
                {
                    var messageBox = new MessageBox("Profile Exists", "This profile already exists.");
                    messageBox.Show(Screen);
                }
            }
        }

        protected override void OnUpdate(TimeSpan deltaTime)
        {
            if (clearText)
            {
                nameBox.Text = "";
                clearText = false;
            }
            base.OnUpdate(deltaTime);
        }
    }
}
