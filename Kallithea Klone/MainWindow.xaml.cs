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

namespace Kallithea_Klone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //  Variables
        //  =========

        private static string RepoFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Kallithea Klone";
        private static string RepoFile = RepoFolder + "\\AllRepositories.dat";

        private static List<string> CheckedURLs = new List<string>();
        private List<string> allRepositories;
        private List<string> errorCodes = new List<string>();

        private string runFrom;
        private bool settingsOpen;
        private int cloningCount;
        private int clonedCount = 0;

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

        //  Constructors
        //  ============

        public MainWindow(string runFrom)
        {
            this.runFrom = runFrom;
            InitializeComponent();

            try
            {
                allRepositories = new List<string>(File.ReadAllLines(RepoFile));
            }
            catch
            {
                if (!Directory.Exists(RepoFolder))
                    Directory.CreateDirectory(RepoFolder);
                File.WriteAllText(RepoFile, "");
                allRepositories = new List<string>();
            }

            LoadRepositories();
        }

        //  Events
        //  ======

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string GetVersion()
            {
                try
                {
                    return Assembly.GetExecutingAssembly().GetName().Version.ToString();
                }
                catch (InvalidDeploymentException)
                {
                    return "not installed";
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
                MessageBox.Show(Password, "", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);

                About about = new About();
                about.ShowDialog();

                settingsOpen = false;
            };

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(MIVersion);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(MIAbout);

            BdrHeader.ContextMenu = contextMenu;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (!settingsOpen)
                Environment.Exit(0);
        }

        private void NewItem_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckedURLs.Remove(((Control)sender).Tag.ToString());
            SelectionUpdated(sender, e);
        }

        private void NewItem_Checked(object sender, RoutedEventArgs e)
        {
            CheckedURLs.Add(((Control)sender).Tag.ToString());
            SelectionUpdated(sender, e);
        }

        private void SelectionUpdated(object sender, RoutedEventArgs e)
        {
            lblNumberSelected.Content = CheckedURLs.Count + " " + (CheckedURLs.Count == 1 ? "Repository" : "Repositories") + " selected";
            BtnClone.IsEnabled = CheckedURLs.Count > 0;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            if (((System.Diagnostics.Process)sender).ExitCode != 0)
                errorCodes.Add(((System.Diagnostics.Process)sender).ExitCode.ToString());
            clonedCount++;

            if (clonedCount == cloningCount)
            {
                if (errorCodes.Count > 0)
                    MessageBox.Show("Finshed, but with the following mercurial exit codes:\n" + string.Join("\n", errorCodes), "Errors", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                Environment.Exit(0);
            }
        }

        private void BtnClone_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidSettings())
            {
               MessageBoxResult result = MessageBox.Show("It looks like you have not properly set up your settings.\n" +
                    "Would you like to open them now?", "Empty settings!", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);

                switch (result)
                {
                    case MessageBoxResult.OK:
                        OpenSettings();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                DisableAll();
                Uri uri = new Uri(Host);

                cloningCount = CheckedURLs.Count;
                foreach (string url in CheckedURLs)
                {
                    string fullURL = $"{uri.Scheme}://{HttpUtility.UrlEncode(Username)}:{HttpUtility.UrlEncode(Password)}@{uri.Host}{uri.PathAndQuery}{url}";

                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                        FileName = "cmd.exe",
                        Arguments = $"/C hg clone {fullURL} \"{runFrom}\\{url.Split('/').Last()}\""
                    };
                    process.StartInfo = startInfo;
                    process.EnableRaisingEvents = true;
                    process.Exited += Process_Exited;

                    process.Start();
                }
            }
        }

        private void BdrHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private async void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidSettings())
            {
                MessageBoxResult result = MessageBox.Show("It looks like you have not properly set up your settings.\n" +
                     "Would you like to open them now?", "Empty settings!", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);

                switch (result)
                {
                    case MessageBoxResult.OK:
                        OpenSettings();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                PbProgress.Visibility = Visibility.Visible;
                PbProgress.IsIndeterminate = true;
                BtnReload.IsEnabled = false;

                await DownloadRepositories();

                PbProgress.Visibility = Visibility.Hidden;
                PbProgress.IsIndeterminate = false;
                BtnReload.IsEnabled = true;
            }
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            OpenSettings();
        }

        private void SearchTermTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.Text.Length == 0)
            {
                //ShowAndCollapse(MainTree);

                LoadRepositories();
            }
            else
            {
                //Filter(MainTree, textBox.Text);
            }
        }

        private void SearchTermTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                TextBox textBox = (TextBox)sender;

                if (textBox.Text.Length != 0)
                {
                    IEnumerable<string> repositories = allRepositories;
                    foreach (string term in textBox.Text.ToLower().Split(' '))
                    {
                        repositories = repositories.Where(r => r.ToLower().Contains(term));
                    }
                    LoadRepositories(repositories.ToList(), true);
                }
            }
        }

        //  Methods
        //  =======

        public async Task DownloadRepositories()
        {
            RestClient client = new RestClient($"{Host}/_admin/api");
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\"id\":\"1\",\"api_key\":\"" + APIKey + "\",\"method\":\"get_repos\",\"args\":{}}", ParameterType.RequestBody);

            try
            {
                //Get the data async
                IRestResponse response = await Task.Run(() =>
                {
                    return client.Execute(request);
                });

                switch (response.ResponseStatus)
                {
                    case ResponseStatus.Completed:
                        Repository[] repos = JsonConvert.DeserializeObject<Response<Repository[]>>(response.Content).Result;

                        if (repos.Length != 0)
                        {
                            if (!Directory.Exists(RepoFolder))
                                Directory.CreateDirectory(RepoFolder);
                            File.WriteAllLines(RepoFile, repos.Select(r => r.URL).ToArray());
                        }

                        allRepositories = repos.Select(r => r.URL).ToList();
                        LoadRepositories();
                        break;
                    case ResponseStatus.TimedOut:
                        MessageBox.Show($"Webrequest to {response.ResponseUri} timed out", "Error!\t\t\t\t", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    case ResponseStatus.Error:
                    case ResponseStatus.Aborted:
                    default:
                        MessageBox.Show("Error: " + response.ErrorMessage, "Uncaught Error!\t\t\t\t", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                        break;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("Error: " + ee.Message, "Uncaught Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
            }
        }

        public void LoadRepositories(List<string> customRepositories = null, bool allCheckboxes = false)
        {
            List<string> repositories = customRepositories ?? allRepositories;

            MainTree.Items.Clear();

            if (allCheckboxes)
            {
                foreach (string location in repositories)
                {
                    CheckBox newItem = new CheckBox
                    {
                        Content = location.Split('/').Last(),
                        Tag = location,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        IsChecked = CheckedURLs.Contains(location)
                    };
                    newItem.Checked += NewItem_Checked;
                    newItem.Unchecked += NewItem_Unchecked;

                    MainTree.Items.Add(newItem);
                }

                return;
            }

            //Create tree of menu items
            Location baseLocation = new Location("Base Location");

            string[] parts;
            foreach (string location in repositories)
            {
                parts = location.Split('/');
                Location current = baseLocation;
                for (int i = 0; i < parts.Length; i++)
                {
                    current = GetOrCreate(current, parts[i]);
                }
            }

            //Sort all children
            SortChildren(baseLocation);

            //Create a treeview node for each location node
            foreach (Location location in baseLocation.InnerLocations)
            {
                CreateTreeViewItem(location);
            }
        }

        private void SortChildren(Location location)
        {
            foreach (Location subLocation in location.InnerLocations)
            {
                SortChildren(subLocation);
            }

            location.InnerLocations = location.InnerLocations.OrderBy(l => l.InnerLocations.Count == 0 ? 1 : 0).ThenBy(l => l.Name).ToList();
        }

        private void CreateTreeViewItem(Location location, TreeViewItem parent = null)
        {
            if (location.InnerLocations.Count == 0)
            {
                string newTag = (parent == null) ? location.Name : parent.Tag + "/" + location.Name;
                CheckBox newItem = new CheckBox
                {
                    Content = location.Name,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Tag = newTag,
                    IsChecked = CheckedURLs.Contains(newTag)
                };
                newItem.Checked += NewItem_Checked;
                newItem.Unchecked += NewItem_Unchecked;

                if (parent == null)
                    MainTree.Items.Add(newItem);
                else
                    parent.Items.Add(newItem);
            }
            else
            {
                TreeViewItem newItem = new TreeViewItem
                {
                    Header = location.Name
                };

                if (parent == null)
                {
                    newItem.Tag = location.Name;
                    MainTree.Items.Add(newItem);
                }
                else
                {
                    newItem.Tag = parent.Tag + "/" + location.Name;
                    parent.Items.Add(newItem);
                }

                foreach (Location subLocation in location.InnerLocations)
                    CreateTreeViewItem(subLocation, newItem);
            }
        }

        private static Location GetOrCreate(Location current, string location)
        {
            if (current.InnerLocations.Count(l => l.Name == location) > 0)
                return current.InnerLocations.FirstOrDefault(l => l.Name == location);
            else
            {
                Location inner = new Location
                {
                    Name = location
                };
                current.InnerLocations.Add(inner);
                return inner;
            }
        }

        private void DisableAll()
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

        private bool ValidSettings()
        {
            return Host != ""
                && APIKey != ""
                && Username != ""
                && Password != "";
        }

        private void OpenSettings()
        {
            settingsOpen = true;
            Settings s = new Settings
            {
                Owner = this
            };
            s.ShowDialog();
            settingsOpen = false;
        }
    }
}
