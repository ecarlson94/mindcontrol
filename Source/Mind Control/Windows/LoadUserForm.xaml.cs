﻿using System;
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
using Emotiv;

namespace Mind_Control.Windows
{
    /// <summary>
    /// Interaction logic for LoadUserForm.xaml
    /// </summary>
    public partial class LoadUserForm : Window
    {
        public LoadUserForm(EmoEngineWrapper engineWrapper)
        {
            InitializeComponent();
        }
    }
}
