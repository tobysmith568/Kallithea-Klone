using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Web;
using Kallithea_Klone.Account_Settings;
using System.Threading.Tasks;
using Kallithea_Klone.Other_Classes;
using System.Collections.Generic;

namespace Kallithea_Klone.States
{
    class UpdateState : LocalEditorState
    {
        //  Variables
        //  =========

        private const string dateTimeFormat = "ddMMyyHHmmss";

        //  Properties
        //  ==========

        public override string Verb => "updating";

        //  Constructors
        //  ============

        public UpdateState(string runLocation) : base(runLocation)
        {

        }

        //  State Methods
        //  =============

        public override MainWindowStartProperties OnLoaded()
        {
            return new MainWindowStartProperties
            {
                Title = "Kallithea Update",
                MainActionContent = "Update",
                ReloadVisibility = Visibility.Hidden
            };
        }
        
        public override async Task OnMainActionAsync(List<Repo> repos)
        {
            foreach (Repo repo in repos)
            {
                try
                {
                    await Update(repo);
                }
                catch (MainActionException e)
                {
                    MessageBox.Show($"Error {Verb} {repo.Name}:\n" + e.Message, $"Error {Verb} {repo.Name}", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        //  Other Methods
        //  =============

        /// <exception cref="Kallithea_Klone.MainActionException"></exception>
        private async Task Update(Repo repo)
        {
            string remotePath = GetDefaultRemotePath(repo.URL);
            Uri uri = new Uri(remotePath);

            string fullURL = $"{uri.Scheme}://{HttpUtility.UrlEncode(AccountSettings.Username)}:{HttpUtility.UrlEncode(AccountSettings.Password)}@{uri.Host}{uri.PathAndQuery}";
            string shelfName = DateTime.Now.ToString(dateTimeFormat);
            CMDProcess cmdProcess = new CMDProcess("UPDATE", repo.Name, new string[]
            {
                    $"cd /d {repo.URL}" +
                    $"hg --config \"extensions.shelve = \" shelve --name {shelfName} {debugArg}" +
                    $"hg pull {fullURL} {debugArg}" +
                    $"hg update -m {debugArg}" +
                    $"hg --config \"extensions.shelve = \" unshelve --name {shelfName} --tool :other {debugArg}"
            });

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
    }
}
