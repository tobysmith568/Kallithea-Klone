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
    public partial class UpdateWindow : Window
    {
        //  Properties
        //  ==========
        
        public string GithubURL { get; }
        public string DownloadURL { get; }

        UpdateFound updateFound;
        UpdateDownloading updateDownloading;

        //  Constructors
        //  ============

        public UpdateWindow(string githubURL, string downloadURL)
        {
            GithubURL = githubURL;
            DownloadURL = downloadURL;

            InitializeComponent();

            updateFound = new UpdateFound(this);
            updateFound.SetValue(Grid.RowProperty, 1);
            updateFound.Margin = new Thickness(0);
            MainGrid.Children.Add(updateFound);

            updateDownloading = new UpdateDownloading(this);
            updateDownloading.SetValue(Grid.RowProperty, 1);
            updateDownloading.Margin = new Thickness(0);
            updateDownloading.Visibility = Visibility.Hidden;
            MainGrid.Children.Add(updateDownloading);
        }

        //  Events
        //  ======

        private void BdrHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //  Methods
        //  =======

        public new void ShowDialog()
        {
            System.Media.SystemSounds.Asterisk.Play();
            base.ShowDialog();
        }

        public void Download()
        {
            updateFound.Visibility = Visibility.Hidden;
            updateDownloading.Visibility = Visibility.Visible;
            updateDownloading.Download();
        }
    }
}
