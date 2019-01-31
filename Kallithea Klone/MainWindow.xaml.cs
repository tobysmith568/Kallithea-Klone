using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Deployment.Application;
using System.Reflection;
using System.Net;
using Kallithea_Klone.States;
using Kallithea_Klone.Other_Classes;
using Kallithea_Klone.Account_Settings;

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

        public static List<string> CheckedURLs = new List<string>();

        private IState state;

        //  Constructors
        //  ============

        /// <exception cref="Exception"></exception>
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

        /// <exception cref="InvalidOperationException">Ignore.</exception>
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
                About about = new About();
                about.ShowDialog();
            };

            System.Windows.Controls.ContextMenu contextMenu = new System.Windows.Controls.ContextMenu();
            contextMenu.Items.Add(MIVersion);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(MIAbout);

            BdrHeader.ContextMenu = contextMenu;

            if (AccountSettings.Updates)
                await CheckForUpdate();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            state.OnLoseFocus();
        }

        /// <exception cref="System.Security.SecurityException">Ignore.</exception>
        private async void BtnClone_Click(object sender, RoutedEventArgs e)
        {
            if (AccountSettings.VerifySettings())
            {
                DisableAll();
                await state.OnMainActionAsync(CheckedURLs);
                Environment.Exit(0);
            }
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
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
            SetEmpty();
        }

        private void SearchTermTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                state.OnSearch();
            }
            SetEmpty();
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
                        PromptUpdate(response);
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
                //No need to do anything
            }
        }

        public void PromptUpdate(IRestResponse response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
                return;

            GithubRelease release = JsonConvert.DeserializeObject<GithubRelease>(response.Content);

            if (release == null)
                return;

            if (release.IsDraft)
                return;

            Version.TryParse(release.Tag.Split('-')[0].Replace("v", ""), out Version version);

            if (Assembly.GetExecutingAssembly().GetName().Version.CompareTo(version) >= 0)
                return;

            Asset asset = release.Assets.FirstOrDefault(r => r.URL.EndsWith(".msi"));

            if (asset == null)
                return;

            UpdateWindow prompt = new UpdateWindow(release.URL, asset.URL)
            {
                Owner = this
            };
            prompt.ShowDialog();
        }

        public void DisableAll()
        {
            Topmost = true;
            GridCoverAll.Visibility = Visibility.Visible;
            Topmost = false;
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

        /// <exception cref="InvalidCastException"></exception>
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
                    throw new InvalidCastException("The mainTreeView may only contain TreeViewItems and Checkboxes.");
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

        public static bool OpenSettings()
        {
            try
            {
                Settings settings = new Settings
                {
                    Owner = singleton
                };
                return settings.ShowDialog() == true;
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Error: Unable to open the settings window!", "Error!",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }

        public void SelectionUpdated()
        {
            lblNumberSelected.Content = CheckedURLs.Count + " " + (CheckedURLs.Count == 1 ? "Repository" : "Repositories") + " selected";
            BtnMainAction.IsEnabled = CheckedURLs.Count > 0;
        }

        public void SetEmpty()
        {
            NoResults.Visibility = MainTree.Items.Count == 0 ? Visibility.Visible : Visibility.Hidden;
        }
    }
}
