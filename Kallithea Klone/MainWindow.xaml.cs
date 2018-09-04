using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;
using System.Web;
using System.Security.Cryptography;
using System.Deployment.Application;
using System.Reflection;
using System.Diagnostics;
using System.Net;

namespace Kallithea_Klone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //  Variables
        //  =========

        public static MainWindow singleton = null;

        public string runFrom;
        public bool settingsOpen;

        public static List<string> CheckedURLs = new List<string>();

        private IState state;

        public static string APIKey
        {
            get
            {
                return Properties.Settings.Default.APIKey;
            }
            set
            {
                Properties.Settings.Default.APIKey = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string Username
        {
            get
            {
                return Properties.Settings.Default.Username;
            }
            set
            {
                Properties.Settings.Default.Username = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string Host
        {
            get
            {
                return Properties.Settings.Default.Host;
            }
            set
            {
                Properties.Settings.Default.Host = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string Password
        {
            get
            {
                if (Properties.Settings.Default.Password == "")
                    return Properties.Settings.Default.Password;

                return Encoding.Unicode.GetString(ProtectedData.Unprotect(
                    Convert.FromBase64String(Properties.Settings.Default.Password),
                    null,
                    DataProtectionScope.LocalMachine));
            }
            set
            {
                Properties.Settings.Default.Password = Convert.ToBase64String(ProtectedData.Protect(
                    Encoding.Unicode.GetBytes(value),
                    null,
                    DataProtectionScope.LocalMachine));
                Properties.Settings.Default.Save();
            }
        }

        public static bool Updates
        {
            get
            {
                return Properties.Settings.Default.CheckForUpdates;
            }
            set
            {
                Properties.Settings.Default.CheckForUpdates = value;
                Properties.Settings.Default.Save();
            }
        }

        //  Constructors
        //  ============

        public MainWindow(RunTypes runType, string runFrom)
        {
            if (singleton == null)
                singleton = this;
            else
                throw new Exception("Cannot create a second mainWindow!");

            state = runType.GetState();

            this.runFrom = runFrom;
            InitializeComponent();

            state.OnLoad();
        }

        //  Events
        //  ======

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            state.OnLoaded();

            string GetVersion()
            {
                try
                {
                    return "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }
                catch (InvalidDeploymentException)
                {
                    return "Not installed";
                }
            }

            MenuItem MIVersion = new MenuItem
            {
                Header = GetVersion(),
                IsEnabled = false,
            };

            MenuItem MIAbout = new MenuItem
            {
                Header = "About"
            };
            MIAbout.Click += (ss, ee) =>
            {
                settingsOpen = true;

                About about = new About();
                about.ShowDialog();

                settingsOpen = false;
            };

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(MIVersion);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(MIAbout);

            BdrHeader.ContextMenu = contextMenu;

            if (Updates)
                await CheckForUpdate();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            state.OnLoseFocus();
        }

        private void BtnClone_Click(object sender, RoutedEventArgs e)
        {
            state.OnMainAction();
        }

        private void BdrHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            state.OnReload();
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            state.OnSettings();
        }

        private void SearchTermTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            state.OnSearchTermChanged();
        }

        private void SearchTermTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                state.OnSearch();
            }
        }

        public void NewItem_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckedURLs.Remove(((Control)sender).Tag.ToString());
            SelectionUpdated();
        }

        public void NewItem_Checked(object sender, RoutedEventArgs e)
        {
            CheckedURLs.Add(((Control)sender).Tag.ToString());
            SelectionUpdated();
        }

        //  Methods
        //  =======

        private async Task CheckForUpdate()
        {
            RestClient client = new RestClient("https://api.tobysmith.uk/github?repo=kallithea-klone");
            RestRequest request = new RestRequest(Method.GET);

            try
            {
                IRestResponse response = await Task.Run(() =>
                {
                    return client.Execute(request);
                });

                switch (response.ResponseStatus)
                {
                    case ResponseStatus.Completed:
                        if (response.StatusCode != HttpStatusCode.OK)
                            return;

                        GithubRelease release = JsonConvert.DeserializeObject<GithubRelease>(response.Content);
                        if (release != null)
                        {
                            if (release.Assets.Count(a => a.URL.EndsWith(".msi")) > 0
                                    && !release.IsDraft
                                    && Version.TryParse(release.Tag.Split('-')[0].Replace("v", ""), out Version version))
                            {
                                if (Assembly.GetExecutingAssembly().GetName().Version.CompareTo(version) < 0)
                                {
                                    MessageBoxResult result = MessageBox.Show("A new version of Kallithea Klone has been found!\n" +
                                        "Do you want to update to it now?", "Update found!",
                                        MessageBoxButton.OKCancel, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);

                                    switch (result)
                                    {
                                        case MessageBoxResult.OK:
                                            Process.Start(new ProcessStartInfo(release.Assets.First(r => r.URL.EndsWith(".msi")).URL));
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                        break;
                    case ResponseStatus.TimedOut:
                    case ResponseStatus.Error:
                    case ResponseStatus.Aborted:
                    default:
                        return;
                }
            }
            catch
            {

            }
        }

        public void DisableAll()
        {
            Topmost = true;
            settingsOpen = true;
            GridCoverAll.Visibility = Visibility.Visible;
        }

        private void ShowAndCollapse(ItemsControl parent)
        {
            foreach (Control control in parent.Items)
            {
                control.Visibility = Visibility.Visible;

                if (control is TreeViewItem treeViewItem)
                {
                    ShowAndCollapse(treeViewItem);

                    treeViewItem.IsExpanded = false;
                }
            }
        }

        private void Filter(ItemsControl parent, string searchTerm)
        {
            foreach (Control child in parent.Items)
            {
                if (child is TreeViewItem treeViewItem)
                {
                    Filter(treeViewItem, searchTerm);

                    if (IsEmpty(treeViewItem))
                    {
                        treeViewItem.IsExpanded = false;
                        treeViewItem.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        treeViewItem.IsExpanded = true;
                        treeViewItem.Visibility = Visibility.Visible;
                    }
                }
                else if (child is CheckBox checkBox)
                {
                    checkBox.Visibility = Visibility.Visible;
                    foreach (string term in searchTerm.ToLower().Split(' '))
                    {
                        if (!checkBox.Tag.ToString().ToLower().Contains(term))
                        {
                            checkBox.Visibility = Visibility.Collapsed;
                            break;
                        }
                    }
                }
                else
                    throw new Exception("Unexpected child type!");
            }
        }

        private bool IsEmpty(TreeViewItem treeViewItem)
        {
            foreach (Control item in treeViewItem.Items)
            {
                if (item.Visibility == Visibility.Visible)
                {
                    return false;
                }
            }
            return true;
        }

        public void OpenSettings()
        {
            settingsOpen = true;
            Settings s = new Settings
            {
                Owner = this
            };
            s.ShowDialog();
            settingsOpen = false;
        }

        public void SelectionUpdated()
        {
            lblNumberSelected.Content = CheckedURLs.Count + " " + (CheckedURLs.Count == 1 ? "Repository" : "Repositories") + " selected";
            BtnClone.IsEnabled = CheckedURLs.Count > 0;
        }
    }
}
