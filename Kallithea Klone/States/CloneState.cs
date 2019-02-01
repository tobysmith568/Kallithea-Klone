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
using Kallithea_Klone.Other_Classes;
using Kallithea_Klone.Kallithea;
using Kallithea_Klone.Account_Settings;
using System.Collections;

namespace Kallithea_Klone.States
{
    public class CloneState : TemplateState
    {
        //  Variables
        //  =========

        private static readonly string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Kallithea Klone");
        private static readonly string allReposFile = Path.Combine(appDataFolder, "AllRepositories.dat");

        private List<string> allRepositories;

        //  Properties
        //  ==========

        public override string Verb => "cloning";

        //  Constructors
        //  ============

        public CloneState()
        {

        }

        //  State Pattern
        //  =============

        public override ICollection<Control> OnLoadRepositories()
        {
            try
            {
                if (!Directory.Exists(appDataFolder))
                    Directory.CreateDirectory(appDataFolder);

                if (!File.Exists(allReposFile))
                {
                    File.WriteAllText(allReposFile, "");
                    allRepositories = new List<string>();
                }
                else
                {
                    allRepositories = new List<string>(File.ReadAllLines(allReposFile));
                }
            }
            catch
            {
                allRepositories = new List<string>();
            }

            return LoadRepositoryTree(allRepositories);
        }

        public override void OnLoaded()
        {

        }

        public override async Task OnMainActionAsync(string localLocation, List<string> urls)
        {
            Uri host = new Uri(AccountSettings.Host);

            foreach (string url in urls)
            {
                try
                {
                    await CloneAsync(localLocation, host, url);
                }
                catch (MainActionException e)
                {
                    MessageBox.Show($"Error {Verb} {Path.GetFileName(url)}:\n" + e.Message, $"Error {Verb} {Path.GetFileName(url)}",
                        MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                }
            }
        }

        public override async void OnReload()
        {
            await DownloadRepositories();
        }

        public override ICollection<Control> OnSearch(string searchTerm)
        {
            IEnumerable<string> repositories = allRepositories;
            foreach (string term in searchTerm.ToLower().Split(' '))
            {
                repositories = repositories.Where(r => r.ToLower().Contains(term));
            }
            return LoadRepositoryList(repositories.ToList());
        }

        public override ICollection<Control> OnSearchCleared(string searchTerm)
        {
            return LoadRepositoryTree(allRepositories);
        }

        //  Other Methods
        //  =============

        /// <exception cref="Kallithea_Klone.MainActionException"></exception>
        public async Task CloneAsync(string localLocation, Uri host, string url)
        {
            string fullURL = $"{host.Scheme}://{HttpUtility.UrlEncode(AccountSettings.Username)}:{HttpUtility.UrlEncode(AccountSettings.Password)}@{host.Host}{host.PathAndQuery}{url}";
            CMDProcess cmdProcess = new CMDProcess($"hg clone {fullURL} \"{localLocation}\\{Path.GetFileName(url)}\"");

            try
            {
                await cmdProcess.Run();
            }
            catch (Exception e)
            {
                throw new MainActionException("Unable to start the necessary command window process", e);
            }

            await ReportErrorsAsync(cmdProcess);
        }

        public ICollection<Control> LoadRepositoryTree(List<string> repositories)
        {
            Location baseLocation = new Location("Base Location");
            
            foreach (string location in repositories)
            {
                Location current = baseLocation;

                foreach (string part in location.Split('/'))
                {
                    current = GetOrCreateChild(current, part);
                }
            }

            SortChildren(baseLocation);

            ICollection<Control> results = new List<Control>();
            foreach (Location location in baseLocation.InnerLocations)
            {
                results.Add(CreateTreeViewItem(location));
            }

            return results;
        }

        public ICollection<Control> LoadRepositoryList(List<string> repositories)
        {
            ICollection<Control> results = new List<Control>();

            foreach (string location in repositories)
            {
                CheckBox newItem = new CheckBox
                {
                    Content = Path.GetFileName(location),
                    Tag = location,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    IsChecked = MainWindow.CheckedURLs.Contains(location)
                };
                newItem.Checked += MainWindow.singleton.NewItem_Checked;
                newItem.Unchecked += MainWindow.singleton.NewItem_Unchecked;

                results.Add(newItem);
            }

            return results;
        }

        private static Location GetOrCreateChild(Location current, string location)
        {
            if (current.InnerLocations.Count(l => l.Name == location) > 0)
            {
                return current.InnerLocations.FirstOrDefault(l => l.Name == location);
            }
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

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        private Control CreateTreeViewItem(Location location, TreeViewItem parent = null)
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
                newItem.Checked += MainWindow.singleton.NewItem_Checked;
                newItem.Unchecked += MainWindow.singleton.NewItem_Unchecked;

                return newItem;
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
                }
                else
                {
                    newItem.Tag = parent.Tag + "/" + location.Name;
                }

                foreach (Location subLocation in location.InnerLocations)
                    newItem.Items.Add(CreateTreeViewItem(subLocation, newItem));

                return newItem;
            }
        }

        public async Task DownloadRepositories()
        {
            RestClient client = new RestClient($"{AccountSettings.Host}/_admin/api");
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\"id\":\"1\",\"api_key\":\"" + AccountSettings.APIKey + "\",\"method\":\"get_repos\",\"args\":{}}", ParameterType.RequestBody);

            try
            {
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
                            if (!Directory.Exists(appDataFolder))
                                Directory.CreateDirectory(appDataFolder);
                            File.WriteAllLines(allReposFile, repos.Select(r => r.URL).ToArray());
                        }

                        allRepositories = repos.Select(r => r.URL).ToList();
                        LoadRepositoryTree(allRepositories);
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
