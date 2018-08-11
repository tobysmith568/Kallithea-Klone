using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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

namespace Kallithea_Klone
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        //  Constructors
        //  ============

        public About()
        {
            InitializeComponent();
            tbVersion.Text = "Version: v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        //  Events
        //  ======

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CbUpdates.IsChecked = MainWindow.Updates;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void BtnClone_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CbUpdates_Toggled(object sender, RoutedEventArgs e)
        {
            MainWindow.Updates = ((CheckBox)sender).IsChecked == true;
        }
    }
}
