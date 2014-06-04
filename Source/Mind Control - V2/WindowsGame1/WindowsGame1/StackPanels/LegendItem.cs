using DigitalRune.Game.UI;
using DigitalRune.Game.UI.Controls;
using DigitalRune.Mathematics.Algebra;

namespace WindowsGame1.StackPanels
{
    public class LegendItem : StackPanel
    {
        private TextBlock _keyName;
        private Image _key;

        public LegendItem(Image key, string keyName)
        {
            _key = key;
            _key.VerticalAlignment = VerticalAlignment.Center;
            _key.HorizontalAlignment = HorizontalAlignment.Center;
            _key.Margin = new Vector4F(3);

            _keyName = new TextBlock
            {
                Text = " - " + keyName,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Vector4F(3)
            };

            Name = keyName;
            Orientation = Orientation.Horizontal;
        }

        protected override void OnLoad()
        {
            Children.Add(_key);
            Children.Add(_keyName);
            base.OnLoad();
        }

        protected override void OnUnload()
        {
            Children.Remove(_key);
            Children.Remove(_keyName);
            base.OnUnload();
        }
    }
}
