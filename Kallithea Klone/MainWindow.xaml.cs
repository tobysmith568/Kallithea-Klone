using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json;
using System.Deployment.Application;
using System.Reflection;
using System.Net;
using Kallithea_Klone.States;
using Kallithea_Klone.Other_Classes;
using Kallithea_Klone.Account_Settings;
using Kallithea_Klone.Github_API;
using System.IO;
using Kallithea_Klone.WPF_Controls;

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

        private IState state;

        private static readonly char[] urlSplitChars = new char[]
        {
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar
        };

        //  Properties
        //  ==========

        public RepositoryFolder LocationTree { get; } = new RepositoryFolder("Base Folder");
        public List<Repository> LocationList { get; } = new List<Repository>();

        public ICollection<RepositoryData> CheckedURLs
        {
            get
            {
                List<RepositoryData> result = new List<RepositoryData>();
                foreach (Repository checkBox in LocationList)
                {
                    if (checkBox.IsChecked == true)
                    {
                        result.Add(checkBox);
                    }
                }
                return result;
            }
        }

        //  Constructors
        //  ============

        /// <exception cref="Exception"></exception>
        public MainWindow(IState state)
        {
            if (singleton == null)
                singleton = this;
            else
                throw new Exception("Cannot create a second mainWindow!");

            this.state = state;

            InitializeComponent();

            LoadRepositories(state.OnLoadRepositories());

            ShowTree();
        }

        //  Events
        //  ======

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindowStartProperties properties = state.OnLoaded();

            if (properties != null)
            {
                LblTitle.Content = properties.Title;
                BtnMainAction.Content = properties.MainActionContent;
                BtnReload.Visibility = properties.ReloadVisibility;
            }

            CreateHeaderContextMenu();

            if (AccountSettings.Updates)
                await CheckForUpdate();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            state.OnLoseFocus(GridCoverAll.Visibility == Visibility.Visible);
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
        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private async void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            if (AccountSettings.VerifySettings())
            {
                PbProgress.Visibility = Visibility.Visible;
                PbProgress.IsIndeterminate = true;
                BtnReload.IsEnabled = false;

                MainTree.ItemsSource = await state.OnReloadAsync();

                PbProgress.Visibility = Visibility.Collapsed;
                PbProgress.IsIndeterminate = false;
                BtnReload.IsEnabled = true;
            }
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            state.OnSettings();
        }

        private void SearchTermTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TbxSearch.Text.Length >= 3)
            {
                ShowList(TbxSearch.Text);
                SetEmpty();
            }
            else if (TbxSearch.Text.Length == 0)
            {
                ShowTree();
                SetEmpty();
            }
        }

        private void SearchTermTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && TbxSearch.Text.Length >= 3)
            {
                ShowList(TbxSearch.Text);
                SetEmpty();
            }
        }

        public void NewItem_Checked(object sender, RoutedEventArgs e)
        {
            SelectionUpdated();
        }

        //  Methods
        //  =======

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        private void CreateHeaderContextMenu()
        {
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
        }

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

        private void LoadRepositories(ICollection<string> collection)
        {
            foreach (string url in state.OnLoadRepositories())
            {
                string[] parts = url.Split(urlSplitChars, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0)
                    return;

                RepositoryFolder currentNode = LocationTree;
                for (int i = 0; i < parts.Length - 1; i++)
                {
                    currentNode = GetOrAddFolder(parts[i], currentNode);
                }

                LocationList.Add(AddRepository(parts[parts.Length - 1], url, currentNode));
            }

            LocationList.Sort((c1, c2) => c1.Content.ToString().CompareTo(c2.Content.ToString()));
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        private RepositoryFolder GetOrAddFolder(string name, RepositoryFolder parent)
        {
            RepositoryFolder result = parent.Items.OfType<RepositoryFolder>().FirstOrDefault(c => c.Header.ToString() == name);

            if (result == null)
            {
                result = new RepositoryFolder(name);

                parent.Items.Add(result);
            }

            return result;
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        private Repository AddRepository(string name, string url, RepositoryFolder parent)
        {
            Repository result = new Repository(name, url, NewItem_Checked);

            parent.Items.Add(result);

            return result;
        }

        private void ShowTree(string searchTerm = null)
        {
            MainTree.ItemsSource = LocationTree.Items;
        }

        private void ShowList(string searchTerm = null)
        {
            IEnumerable<Repository> results = LocationList;

            if (searchTerm != null)
            {
                foreach (string word in searchTerm.Split(' '))
                {
                    results = results.Where(c => c.RepoURL.IndexOf(word, StringComparison.CurrentCultureIgnoreCase) >= 0);
                }
            }

            MainTree.ItemsSource = results;
        }
    }
}
