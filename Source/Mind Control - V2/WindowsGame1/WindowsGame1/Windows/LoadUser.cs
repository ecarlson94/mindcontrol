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
            CloseButtonStyle = "";
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
            //find way to override update() method
            stackPanel.Children.Add(loadPanel);

            profileList = new DropDownButton
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Vector4F(3),
                MaxDropDownHeight = 250,
            };
            loadPanel.Children.Add(profileList);

            string[] profileNames = emoEngine.GetProfileNames();
            profileList.SelectedIndex = 0;
            for (int i = 0; i < profileNames.Length; i++)
            {
                profileList.Items.Add(profileNames[i]);
                if (profileNames[i].Length >= profileNames[profileList.SelectedIndex].Length)
                {
                    profileList.SelectedIndex = i;
                }
            }
            

            var loadButton = new Button
            {
                Name = "LoadUser",
                Content = new TextBlock {Text = "Load User"},
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Vector4F(3),
                Focusable = false,
                FocusWhenMouseOver = false,
            };
            loadButton.Click += (s, e) =>
            {
                string profileName = (string)profileList.Items[profileList.SelectedIndex];
                emoEngine.LoadProfile(profileName);
                Close();
            };
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
                Focusable = false,
                FocusWhenMouseOver = false,
            };
            createButton.Click += (s, e) =>
            {
                Window createUser = new CreateUser(emoEngine);
                Screen.Children.Add(createUser);
                Close();
            };
            createPanel.Children.Add(createButton);

            var deleteButton = new Button
            {
                Name = "DeleteUser",
                Content = new TextBlock{Text = "Delete User"},
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Vector4F(3),
                FocusWhenMouseOver = false,
                Focusable = false,
            };
            deleteButton.Click += (s, e) =>
            {
                string profileName = (string)profileList.Items[profileList.SelectedIndex];
                emoEngine.DeleteProfile(profileName);

                loadPanel.Children.Remove(profileList);
                profileNames = emoEngine.GetProfileNames();
                profileList.SelectedIndex = 0;
                profileList.Items.Clear();
                for (int i = 0; i < profileNames.Length; i++)
                {
                    profileList.Items.Add(profileNames[i]);
                    if (profileNames[i].Length >= profileNames[profileList.SelectedIndex].Length)
                    {
                        profileList.SelectedIndex = i;
                    }
                }
                loadPanel.Children.Insert(0, profileList);
            };
            createPanel.Children.Add(deleteButton);
        }
    }
}
