using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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

namespace Kallithea_Klone
{
    /// <summary>
    /// Interaction logic for UpdateDownloading.xaml
    /// </summary>
    public partial class UpdateDownloading : UserControl
    {

        //  Variables
        //  =========

        private UpdateWindow parent;
        private string downloadFolder;
        private string downloadLocation;

        //  Constructors
        //  ============

        public UpdateDownloading(UpdateWindow parent)
        {
            this.parent = parent;
            downloadFolder = System.IO.Path.GetTempPath() + "Kallithea Klone";
            downloadLocation = downloadFolder + "\\Update.msi";

            InitializeComponent();
        }

        //  Events
        //  ======

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            parent.Close();
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            pbDownloadingProgress.Value = e.ProgressPercentage;
        }

        private void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                BtnInstall.Visibility = Visibility.Visible;
                lblActivity.Content = "Done!";
            }
            else if (e.Error != null)
            {
                MessageBoxResult result = MessageBox.Show("Unable to download the update!\nWould you like to download it in your web browser instead?", "Download Failure", MessageBoxButton.OKCancel, MessageBoxImage.Error, MessageBoxResult.OK);
                switch (result)
                {
                    case MessageBoxResult.OK:
                        Process.Start(parent.GithubURL);
                        Environment.Exit(0);
                        break;
                    default:
                        parent.Close();
                        break;
                }
            }
        }

        private void BtnInstall_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(downloadLocation);
            Environment.Exit(0);
        }

        //  Methods
        //  =======

        public void Download()
        {
            Directory.CreateDirectory(downloadFolder);

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (WebClient client = new WebClient())
            {
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                client.DownloadProgressChanged += Client_DownloadProgressChanged;
                client.DownloadFileAsync(new Uri(parent.DownloadURL), downloadLocation);
            }
        }
    }
}