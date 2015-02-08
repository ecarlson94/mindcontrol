using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;

namespace WindowsGame1.Windows
{
    public class MessageBox : Window
    {
        public MessageBox(string message) : this("",message){}
        public MessageBox(string title, string message)
        {
            Title = title;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;
            HideOnClose = false;

            var stackPanel = new StackPanel
            {
                Name = "MessagePanel",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Vertical,
            };
            Content = stackPanel;

            stackPanel.Children.Add(new TextBlock
            {
                Name = "Message",
                Text = message,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Vector4F(4),
                WrapText = true,
                MaxWidth = 250,
            });

            var okButton = new Button
            {
                Name = "OkButton",
                Content = new TextBlock{Text = "Ok"},
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Vector4F(4),
                Focusable = false,
                FocusWhenMouseOver = false,
            };
            okButton.Click += (s, e) => Close();

            stackPanel.Children.Add(okButton);
        }
    }
}
