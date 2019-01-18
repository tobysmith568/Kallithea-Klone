using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.Web;
using System.Diagnostics;
using Kallithea_Klone.Other_Classes;

namespace Kallithea_Klone.States
{
    public class CloneState : TemplateState
    {
        //  Variables
        //  =========

        private static string RepoFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Kallithea Klone";
        private static string RepoFile = RepoFolder + "\\AllRepositories.dat";

        private List<string> allRepositories;

        private int cloningCount;
        private int clonedCount = 0;
        private List<string> errorCodes = new List<string>();

        //  Constructors
        //  ============

        public CloneState() : base()
        {

        }

        //  Events
        //  ======

        private void Process_Exited(object sender, EventArgs e)
        {
            if (((Process)sender).ExitCode != 0)
                errorCodes.Add(((Process)sender).ExitCode.ToString());
            clonedCount++;

            if (clonedCount == cloningCount)
            {
                if (errorCodes.Count > 0)
                    MessageBox.Show("Finshed, but with the following mercurial exit codes:\n" + string.Join("\n", errorCodes), "Errors", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                Environment.Exit(0);
            }
        }

        //  State Pattern
        //  =============

        public override void OnLoad()
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
                allRepositories = new List<string>();
            }

            LoadRepositories();
        }

        public override void OnMainAction()
        {
            if (!ValidSettings())
            {
                MessageBoxResult result = MessageBox.Show("It looks like you have not properly set up your settings.\n" +
                     "Would you like to open them now?", "Empty settings!", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);

                switch (result)
                {
                    case MessageBoxResult.OK:
                        mainWindow.OpenSettings();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                mainWindow.DisableAll();
                Uri uri = new Uri(MainWindow.Host);

                cloningCount = MainWindow.CheckedURLs.Count;
                foreach (string url in MainWindow.CheckedURLs)
                {
                    string fullURL = $"{uri.Scheme}://{HttpUtility.UrlEncode(MainWindow.Username)}:{HttpUtility.UrlEncode(MainWindow.Password)}@{uri.Host}{uri.PathAndQuery}{url}";

                    Process process = new Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        FileName = CmdExe,
                        Arguments = $"/C hg clone {fullURL} \"{mainWindow.runFrom}\\{url.Split('/').Last()}\""
                    };
                    process.StartInfo = startInfo;
                    process.EnableRaisingEvents = true;
                    process.Exited += Process_Exited;

                    process.Start();
                }
            }
        }

        public override async void OnReload()
        {
            if (!ValidSettings())
            {
                MessageBoxResult result = MessageBox.Show("It looks like you have not properly set up your settings.\n" +
                     "Would you like to open them now?", "Empty settings!", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);

                switch (result)
                {
                    case MessageBoxResult.OK:
                        mainWindow.OpenSettings();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                mainWindow.PbProgress.Visibility = Visibility.Visible;
                mainWindow.PbProgress.IsIndeterminate = true;
                mainWindow.BtnReload.IsEnabled = false;

                await DownloadRepositories();

                mainWindow.PbProgress.Visibility = Visibility.Hidden;
                mainWindow.PbProgress.IsIndeterminate = false;
                mainWindow.BtnReload.IsEnabled = true;
            }
        }

        public override void OnSearchTermChanged()
        {
            if (mainWindow.TbxSearch.Text.Length == 0)
            {
                LoadRepositories();
            }
        }

        public override void OnSearch()
        {
            if (mainWindow.TbxSearch.Text.Length != 0)
            {
                IEnumerable<string> repositories = allRepositories;
                foreach (string term in mainWindow.TbxSearch.Text.ToLower().Split(' '))
                {
                    repositories = repositories.Where(r => r.ToLower().Contains(term));
                }
                LoadRepositories(repositories.ToList(), true);
            }
        }

        //  Other Methods
        //  =============

        public void LoadRepositories(List<string> customRepositories = null, bool allCheckboxes = false)
        {
            List<string> repositories = customRepositories ?? allRepositories;

            mainWindow.MainTree.Items.Clear();

            if (allCheckboxes)
            {
                foreach (string location in repositories)
                {
                    CheckBox newItem = new CheckBox
                    {
                        Content = location.Split('/').Last(),
                        Tag = location,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        IsChecked = MainWindow.CheckedURLs.Contains(location)
                    };
                    newItem.Checked += mainWindow.NewItem_Checked;
                    newItem.Unchecked += mainWindow.NewItem_Unchecked;

                    mainWindow.MainTree.Items.Add(newItem);
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
                    IsChecked = MainWindow.CheckedURLs.Contains(newTag)
                };
                newItem.Checked += mainWindow.NewItem_Checked;
                newItem.Unchecked += mainWindow.NewItem_Unchecked;

                if (parent == null)
                    mainWindow.MainTree.Items.Add(newItem);
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
                    mainWindow.MainTree.Items.Add(newItem);
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

        public async Task DownloadRepositories()
        {
            RestClient client = new RestClient($"{MainWindow.Host}/_admin/api");
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\"id\":\"1\",\"api_key\":\"" + MainWindow.APIKey + "\",\"method\":\"get_repos\",\"args\":{}}", ParameterType.RequestBody);

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
                        Repository[] repos = JsonConvert.DeserializeObject<KallitheaResponse<Repository[]>>(response.Content).Result;

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
    }
}
