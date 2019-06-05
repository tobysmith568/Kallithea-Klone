using KallitheaKlone.Models.Dialogs.MessagePrompts;
using KallitheaKlone.Models.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KallitheaKlone.WPF.Models.Repositories
{
    public class RepositoryManager : IRepositoryManager<Repository>
    {
        //  Constants
        //  =========

        private const string AppDataFolderName = "Kallithea Klone";
        private const string RepositoryFileName = "AppRepositories.dat";

        //  Variables
        //  =========

        private static readonly string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataFolderName);
        private static readonly string allReposFile = Path.Combine(appDataFolder, RepositoryFileName);
        private static readonly string nl = Environment.NewLine;

        private readonly IMessagePrompt messagePrompt;

        //  Constructors
        //  ============

        public RepositoryManager(IMessagePrompt messagePrompt)
        {
            this.messagePrompt = messagePrompt;
        }

        //  Methods
        //  =======

        public async Task<ICollection<Repository>> GetAllRepositories()
        {
            return await Task.Run(() => AsyncImplementation());

            ICollection<Repository> AsyncImplementation()
            {
                try
                {
                    if (!Directory.Exists(appDataFolder))
                    {
                        Directory.CreateDirectory(appDataFolder);
                    }

                    if (!File.Exists(allReposFile))
                    {
                        File.WriteAllText(allReposFile, string.Empty);
                        return new List<Repository>(0);
                    }
                    else
                    {
                        string[] repoURLs = File.ReadAllLines(allReposFile);
                        ICollection<Repository> result = new List<Repository>(repoURLs.Length);

                        foreach (string repoURL in repoURLs)
                        {
                            result.Add(new Repository()
                            {
                                URL = repoURL
                            });
                        }

                        return result;
                    }
                }
                catch
                {
                    messagePrompt.PromptOK($"Unable to read repositories from storage!{nl}" +
                                           $"Please reload your repositories.", "Error", MessageType.Error);

                    return new List<Repository>(0);
                }
            }
        }

        public Task<bool> OverwriteAllRespositories(ICollection<Repository> _repositories)
        {
            _repositories = _repositories ?? new List<Repository>(0);

            return Task.Run(() => AsyncImplementation(_repositories));

            bool AsyncImplementation(ICollection<Repository> repositories)
            {
                try
                {

                    if (!Directory.Exists(appDataFolder))
                    {
                        Directory.CreateDirectory(appDataFolder);
                    }

                    File.WriteAllLines(allReposFile, repositories.Select(r => r.URL));

                    return true;
                }
                catch
                {
                    messagePrompt.PromptOK($"Unable to save repositories to storage!{nl}" +
                                           $"Please reopen the program and try again.", "Error", MessageType.Error);

                    return false;
                }
            }
        }
    }
}