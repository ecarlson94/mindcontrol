using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;

namespace WindowsGame1.Windows
{
    public class OptionDialog : Window
    {
        public OptionDialog(string message, string title = "", string yesOption = "Yes", string noOption = "No")
        {
            Title = title;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;
            HideOnClose = false;
            CloseButtonStyle = "";

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

            var dialogOptions = new StackPanel
            {
                Name = "DialogOptions",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Horizontal,
            };
            stackPanel.Children.Add(dialogOptions);

            var yesButton = new Button
            {
                Name = "YesButton",
                Content = new TextBlock { Text = yesOption },
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Vector4F(4),
                Focusable = false,
                FocusWhenMouseOver = false,
            };
            yesButton.Click += (s, e) =>
            {
                DialogResult = true;
                Close();
            };
            dialogOptions.Children.Add(yesButton);

            var noButton = new Button
            {
                Name = "OkButton",
                Content = new TextBlock { Text = noOption },
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Vector4F(4),
                Focusable = false,
                FocusWhenMouseOver = false,
            };
            noButton.Click += (s, e) =>
            {
                DialogResult = false;
                Close();
            };
            dialogOptions.Children.Add(noButton);
        }
    }
}
