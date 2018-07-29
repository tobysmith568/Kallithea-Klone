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
using static Kallithea_Klone.Properties.Settings;

namespace Kallithea_Klone
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Default.APIKey = TbxAPIKey.Text;
            Default.Host = TbxHost.Text;
            Default.Email = TbxEmail.Text;
            Default.Password = TbxPassword.Text;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TbxAPIKey.Text = Default.APIKey;
            TbxHost.Text = Default.Host;
            TbxEmail.Text = Default.Email;
            TbxPassword.Text = Default.Password;
        }
    }
}
