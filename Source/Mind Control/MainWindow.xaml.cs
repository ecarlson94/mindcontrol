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

namespace Mind_Control
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EmoEngine emoEngine;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer dt = new DispatcherTimer();
            dt.Tick += dispatcherTimer_OnTick;
            dt.Interval = new TimeSpan(0, 0, 1);
            dt.Start();

            emoEngine = EmoEngine.Instance;
            emoEngine.EmoStateUpdated += engine_EmoStateUpdated;
            emoEngine.Connect();
        }

        private void dispatcherTimer_OnTick(object sender, EventArgs e)
        {
            try
            {
                IntPtr es = EdkDll.EE_EmoStateCreate();
                HeadsetOnTextBox.Text = Convert.ToString(EdkDll.ES_GetHeadsetOn(es));
                //emoEngine.ProcessEvents(1000);
            }
            catch(EmoEngineException ex)
            {
                MessageBox.Show((ex.Message));
            }
        }

        private void engine_EmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {
            EmoState es = e.emoState;

            int nExcitementScore = es.GetHeadsetOn();
            HeadsetOnTextBox.Text = Convert.ToString(nExcitementScore);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            emoEngine.Disconnect();
            emoEngine = null;
        }
    }
}
