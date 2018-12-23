using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Kallithea_Klone
{
    /// <summary>
    /// Interaction logic for UpdatePrompt.xaml
    /// </summary>
    public partial class UpdatePrompt : Window
    {
        //  Variables
        //  =========
        
        private string githubURL;
        private string downloadURL;

        //  Constructors
        //  ============

        public UpdatePrompt(string githubURL, string downloadURL)
        {
            this.githubURL = githubURL;
            this.downloadURL = downloadURL;

            InitializeComponent();
        }

        //  Events
        //  ======

        private void BtnGithub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(githubURL));
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(downloadURL));
            Environment.Exit(0);
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BdrHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
