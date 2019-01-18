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
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void BtnClone_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
