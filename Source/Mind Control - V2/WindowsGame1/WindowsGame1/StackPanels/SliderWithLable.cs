using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;

namespace WindowsGame1.StackPanels
{
    public class SliderWithLable : StackPanel
    {
        public Slider Slider { get; private set; }

        public SliderWithLable(string nameParam)
        {
            Name = nameParam;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Stretch;
            Orientation = Orientation.Vertical;
            Margin = new Vector4F(30);
            RenderScale = new Vector2F(1.5f);

            Initialize();
        }

        private void Initialize()
        {
            TopPanel();
            BottomPanel();
        }

        private void TopPanel()
        {
            var topPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Horizontal,
            };
            Children.Add(topPanel);

            var name = new TextBlock
            {
                Text = Name,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Vector4F(3),
            };
            topPanel.Children.Add(name);

            //add image here
            //figure out spacing
        }

        private void BottomPanel()
        {
            Slider = new Slider
            {
                Margin = new Vector4F(4),
                //RenderScale = new Vector2F(1.50f),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Focusable = false,
                FocusWhenMouseOver = false,
            };
            Children.Add(Slider);
        }
    }
}
