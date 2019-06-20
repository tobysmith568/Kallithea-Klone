using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;

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

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            }
            catch (Exception)
            {
                MessageBox.Show($"Unable to open URL: {e.Uri.AbsoluteUri}", "Error opening link", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            e.Handled = true;
        }

        private void BtnClone_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
