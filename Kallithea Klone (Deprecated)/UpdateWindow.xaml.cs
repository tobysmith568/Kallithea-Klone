using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Kallithea_Klone
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
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

        /// <exception cref="System.InvalidOperationException">Ignore.</exception>
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

        /// <exception cref="System.InvalidOperationException">Ignore.</exception>
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

        /// <exception cref="System.InvalidOperationException">Ignore.</exception>
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
