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
    public class CloneState : ActionState
    {
        //  Variables
        //  =========

        private static readonly string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Kallithea Klone");
        private static readonly string allReposFile = Path.Combine(appDataFolder, "AllRepositories.dat");

        private ICollection<string> allRepositories;

        //  Properties
        //  ==========

        public override string Verb => "cloning";

        //  Constructors
        //  ============

        public CloneState(string runLocation) : base(runLocation)
        {
        }

        //  State Methods
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

        public override MainWindowStartProperties OnLoaded()
        {
            return null;
        }

        public override async Task OnMainActionAsync(List<string> urls)
        {
            Uri host = new Uri(AccountSettings.Host);

            foreach (string url in urls)
            {
                try
                {
                    await CloneAsync(host, url);
                }
                catch (MainActionException e)
                {
                    MessageBox.Show($"Error {Verb} {Path.GetFileName(url)}:\n" + e.Message, $"Error {Verb} {Path.GetFileName(url)}",
                        MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                }
            }
        }

        public override async Task OnReloadAsync()
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
        public async Task CloneAsync(Uri host, string url)
        {
            string fullURL = $"{host.Scheme}://{HttpUtility.UrlEncode(AccountSettings.Username)}:{HttpUtility.UrlEncode(AccountSettings.Password)}@{host.Host}{host.PathAndQuery}{url}";
            CMDProcess cmdProcess = new CMDProcess($"hg clone {fullURL} \"{RunLocation}\\{Path.GetFileName(url)}\"");

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
