using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Emotiv;
using Mind_Control.Windows;
using Mind_Control.Wrappers;

namespace Mind_Control
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EmoEngineWrapper engineWrapper;
        private LoadUserForm loadUser;
        private CreateUserForm createUser;

        public MainWindow()
        {
            InitializeComponent();

            InitializeEmoEngine();
        }

        private void InitializeLoadUserForm()
        {
            loadUser = new LoadUserForm(engineWrapper)
            {
                Title = "Load User",
                ShowInTaskbar = false,
                Topmost = true,
                ResizeMode = ResizeMode.NoResize,
                Owner = Application.Current.MainWindow,
            };
        }

        private void InitializeCreateUserForm()
        {
            createUser = new CreateUserForm(engineWrapper)
            {
                Title = "Create User",
                ShowInTaskbar = false,
                Topmost = true,
                ResizeMode = ResizeMode.NoResize,
                Owner = Application.Current.MainWindow,
            };
        }

        private void InitializeEmoEngine()
        {
            engineWrapper = FindResource("emoEngineWrapper") as EmoEngineWrapper;
            DataContext = engineWrapper;

            engineWrapper.UserAdded += Toggle_DonglePlugged;
            engineWrapper.UserRemoved += Toggle_DongleUnPlugged;

            engineWrapper.StartEmoEngine();
        }

        private void Toggle_DonglePlugged(object sender, EmoEngineEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action) delegate()
            {
                DongleLabel.Content = "Dongle is plugged in.";
                if (engineWrapper.GetProfileNames().Length > 0)
                {
                    InitializeLoadUserForm();
                    loadUser.ShowDialog();
                }
                else
                {
                    InitializeCreateUserForm();
                    createUser.ShowDialog();
                }
            });
        }

        private void Toggle_DongleUnPlugged(object sender, EmoEngineEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action) delegate()
            {
                DongleLabel.Content = "Please plug in the Emotiv Bluetooth Dongle.";
                if (loadUser != null && loadUser.IsActive)
                {
                    loadUser.Close();
                }
                if (createUser != null && createUser.IsActive)
                {
                    createUser.Close();
                }
            });
        }
    }
}