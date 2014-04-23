using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Mind_Control.Wrappers;

namespace Mind_Control.Windows
{
    /// <summary>
    /// Interaction logic for CreateUserForm.xaml
    /// </summary>
    public partial class CreateUserForm : Window
    {
        private EmoEngineWrapper emoEngine;
        public CreateUserForm(EmoEngineWrapper engineWrapper)
        {
            emoEngine = engineWrapper;
            InitializeComponent();
        }

        private void NameBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (NameBox.Text.Length > 25)
                NameBox.Text = NameBox.Text.Substring(0, 25);
        }

        private void NameBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            if(NameBox.Text.EndsWith("New User"))
                NameBox.Text = "";
        }

        private void CreateUser_OnClick(object sender, RoutedEventArgs e)
        {
            if (emoEngine.GetProfileNames().Contains(NameBox.Text))
                MessageBox.Show("That User Already Exists");
            else
                emoEngine.SaveProfile(NameBox.Text);
        }
    }
}
