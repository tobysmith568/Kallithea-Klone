using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Windows.Media.Animation;
using System.Web;
using System.Net;
using System.Security.Cryptography;

namespace Kallithea_Klone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static List<string> CheckedURLs = new List<string>();
        private static string RepoFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Kallithea Klone";
        private static string RepoFile = RepoFolder + "\\AllRepositories.dat";

        private string runFrom;
        private bool settingsOpen;
        private int cloningCount;
        private int clonedCount = 0;
        private List<string> errorCodes = new List<string>();

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

        public static string Email
        {
            get
            {
                return Properties.Settings.Default.Email;
            }
            set
            {
                Properties.Settings.Default.Email = value;
                Properties.Settings.Default.Save();
            }
        }

        public static string Password
        {
            get
            {
                return Encoding.Unicode.GetString(ProtectedData.Unprotect(
                    Convert.FromBase64String(Properties.Settings.Default.Password),
                    null,
                    DataProtectionScope.CurrentUser));
            }
            set
            {
                Properties.Settings.Default.Password = Convert.ToBase64String(ProtectedData.Protect(
                    Encoding.Unicode.GetBytes(value),
                    null,
                    DataProtectionScope.CurrentUser));
                Properties.Settings.Default.Save();
            }
        }

        public MainWindow(string runFrom)
        {
            Password = "hello";
            this.runFrom = runFrom;
            InitializeComponent();
            LoadRepositories();
        }

        public async Task DownloadRepositories()
        {
            RestClient client = new RestClient($"http://{Host}/_admin/api");
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
                        Repository[] repos = JsonConvert.DeserializeObject<Response>(response.Content).Repositories;

                        if (repos.Length != 0)
                        {
                            if (!Directory.Exists(RepoFolder))
                                Directory.CreateDirectory(RepoFolder);
                            File.WriteAllLines(RepoFile, repos.Select(r => r.URL).ToArray());
                        }

                        LoadRepositories(repos.Select(r => r.URL).ToList());
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

        public void LoadRepositories(List<string> defaultRepositories = null)
        {
            List<string> allRepositories = new List<string>();

            if (defaultRepositories == null)
            {
                try
                {
                    allRepositories = new List<string>(File.ReadAllLines(RepoFile));
                }
                catch
                {
                    if (!Directory.Exists(RepoFolder))
                        Directory.CreateDirectory(RepoFolder);
                    File.WriteAllText(RepoFile, "");
                }
            }
            else
                allRepositories = defaultRepositories;

            //Create tree of menu items
            Location baseLocation = new Location("Base Location");

            string[] parts;
            foreach (string location in allRepositories)
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

            MainTree.Items.Clear();

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
                CheckBox newItem = new CheckBox
                {
                    Content = location.Name,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                newItem.Checked += NewItem_Checked;
                newItem.Unchecked += NewItem_Unchecked;

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
            }
            else
            {
                TreeViewItem newItem = new TreeViewItem
                {
                    Header = location.Name,
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

        private void Window_Deactivated(object sender, EventArgs e)
        {
            if (!settingsOpen)
                Environment.Exit(0);
        }

        private void BtnClone_Click(object sender, RoutedEventArgs e)
        {
            DisableAll();

            Console.WriteLine("Cloning:\n" + string.Join("\n", CheckedURLs.Select(u => string.Concat($"http://{HttpUtility.UrlEncode(Email)}@{Host}/", u)).ToArray()));

            string[] cloneURLs = CheckedURLs.Select(u => string.Concat($"http://{HttpUtility.UrlEncode(Email)}:{HttpUtility.UrlEncode(Password)}@{Host}/", u)).ToArray();
            cloningCount = cloneURLs.Length;
            foreach (string url in cloneURLs)
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = $"/C hg clone {url} \"{runFrom}\\{url.Split('/').Last()}\""
                };
                process.StartInfo = startInfo;
                process.EnableRaisingEvents = true;
                process.Exited += Process_Exited;

                process.Start();
            }
        }

        private void DisableAll()
        {
            Topmost = true;
            settingsOpen = true;
            GridCoverAll.Visibility = Visibility.Visible;
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

        private void BdrHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private async void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            PbProgress.Visibility = Visibility.Visible;
            PbProgress.IsIndeterminate = true;
            BtnReload.IsEnabled = false;

            await DownloadRepositories();

            PbProgress.Visibility = Visibility.Hidden;
            PbProgress.IsIndeterminate = false;
            BtnReload.IsEnabled = true;
        }

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            settingsOpen = true;
            Settings s = new Settings();
            s.ShowDialog();
            settingsOpen = false;
        }
    }
}
