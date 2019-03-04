﻿using RestSharp;
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

namespace Kallithea_Klone.States
{
    public class CloneState : ActionState
    {
        //  Variables
        //  =========

        private static readonly string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Kallithea Klone");
        private static readonly string allReposFile = Path.Combine(appDataFolder, "AllRepositories.dat");

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
        
        public override ICollection<string> OnLoadRepositories()
        {
            try
            {
                if (!Directory.Exists(appDataFolder))
                    Directory.CreateDirectory(appDataFolder);

                if (!File.Exists(allReposFile))
                {
                    File.WriteAllText(allReposFile, "");
                    return new string[0];
                }
                else
                {
                    return File.ReadAllLines(allReposFile);
                }
            }
            catch
            {
                return new string[0];
            }
        }

        public override MainWindowStartProperties OnLoaded()
        {
            return null;
        }

        public override async Task OnMainActionAsync(ICollection<RepositoryData> repositories)
        {
            Uri host = new Uri(AccountSettings.Host);

            foreach (RepositoryData repository in repositories)
            {
                try
                {
                    await CloneAsync(host, repository);
                }
                catch (MainActionException e)
                {
                    MessageBox.Show($"Error {Verb} {repository.Name}:\n" + e.Message, $"Error {Verb} {Path.GetFileName(repository.URL)}",
                        MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                }
            }
        }

        public override async Task<ICollection<string>> OnReloadAsync()
        {
            return await DownloadRepositories();
        }

        //  Other Methods
        //  =============

        /// <exception cref="Kallithea_Klone.MainActionException"></exception>
        public async Task CloneAsync(Uri host, RepositoryData location)
        {
            string fullURL = $"{host.Scheme}://{HttpUtility.UrlEncode(AccountSettings.Username)}:{HttpUtility.UrlEncode(AccountSettings.Password)}@{host.Host}{host.PathAndQuery}{location.URL}";
            CMDProcess cmdProcess = new CMDProcess("CLONE", location.Name, $"hg clone {fullURL} \"{RunLocation}\\{location.Name}\" {debugArg}");

            try
            {
                await cmdProcess.Run();
            }
            catch (Exception e)
            {
                throw new MainActionException("Unable to start the necessary command window process", e);
            }

            cmdProcess.ReportErrorsAsync(Verb);
        }

        public async Task<ICollection<string>> DownloadRepositories()
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

                        return repos.Select(r => r.URL).ToArray();
                    case ResponseStatus.TimedOut:
                        MessageBox.Show($"Webrequest to {response.ResponseUri} timed out", "Error!\t\t\t\t", MessageBoxButton.OK, MessageBoxImage.Error);
                        return null;
                    case ResponseStatus.Error:
                    case ResponseStatus.Aborted:
                    default:
                        MessageBox.Show("Error: " + response.ErrorMessage, "Uncaught Error!\t\t\t\t", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                        return null;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("Error: " + ee.Message, "Uncaught Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                return null;
            }
        }
    }
}
