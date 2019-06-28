using KallitheaKlone.Models.Dialogs.MessagePrompts;
using KallitheaKlone.Models.JSONConverter;
using KallitheaKlone.Models.Repositories.RemoteRepositories;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KallitheaKlone.WPF.Models.Repositories.RemoteRepositories
{
    public class RepositoryManager : IRepositoryManager<RepositoryFolder, Repository>
    {
        //  Constants
        //  =========

        private const string AppDataFolderName = "Kallithea Klone";
        private const string RepositoryFileName = "KallitheaRepositories.dat";

        //  Variables
        //  =========

        private static readonly string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataFolderName);
        private static readonly string allRepositoriesFile = Path.Combine(appDataFolder, RepositoryFileName);
        private static readonly string nl = Environment.NewLine;

        private readonly IMessagePrompt messagePrompt;
        private readonly IJSONConverter jsonConverter;

        //  Constructors
        //  ============

        public RepositoryManager(IMessagePrompt messagePrompt, IJSONConverter jsonConverter)
        {
            this.messagePrompt = messagePrompt;
            this.jsonConverter = jsonConverter;
        }

        //  Methods
        //  =======

        public async Task<IRepositoryFolder<RepositoryFolder, Repository>> GetAllRepositories()
        {
            return await Task.Run(() => AsyncImplementation());

            RepositoryFolder AsyncImplementation()
            {
                try
                {
                    EnsureDirectoryExists();

                    if (!File.Exists(allRepositoriesFile))
                    {
                        File.WriteAllText(allRepositoriesFile, string.Empty);
                        return new RepositoryFolder();
                    }

                    string fileData = File.ReadAllText(allRepositoriesFile);

                    RepositoryFolder result = jsonConverter.FromJson<RepositoryFolder>(fileData);

                    return result;
                }
                catch
                {
                    messagePrompt.PromptOK($"Unable to read repositories from storage!{nl}" +
                                           $"Please reload your repositories.", "Error", MessageType.Error);

                    return new RepositoryFolder();
                }
            }
        }

        public Task<bool> OverwriteAllRespositories(IRepositoryFolder<RepositoryFolder, Repository> _baseRepository)
        {
            _baseRepository = _baseRepository ?? new RepositoryFolder();

            return Task.Run(() => AsyncImplementation(_baseRepository));

            bool AsyncImplementation(IRepositoryFolder<RepositoryFolder, Repository> baseRepository)
            {
                try
                {
                    EnsureDirectoryExists();

                    string fileData = jsonConverter.ToJson(baseRepository);

                    File.WriteAllText(allRepositoriesFile, fileData);

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

        /// <exception cref="IOException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }
        }
    }
}