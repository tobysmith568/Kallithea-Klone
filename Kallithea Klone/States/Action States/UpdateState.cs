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
        
        public override async Task OnMainActionAsync(List<string> urls)
        {
            foreach (string url in urls)
            {
                try
                {
                    await Update(url);
                }
                catch (MainActionException e)
                {
                    MessageBox.Show($"Error {Verb} {Path.GetFileName(url)}:\n" + e.Message, $"Error {Verb} {Path.GetFileName(url)}", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        //  Other Methods
        //  =============

        /// <exception cref="Kallithea_Klone.MainActionException"></exception>
        private async Task Update(string url)
        {
            string remotePath = GetDefaultRemotePath(url);
            Uri uri = new Uri(remotePath);

            string fullURL = $"{uri.Scheme}://{HttpUtility.UrlEncode(AccountSettings.Username)}:{HttpUtility.UrlEncode(AccountSettings.Password)}@{uri.Host}{uri.PathAndQuery}";
            string shelfName = DateTime.Now.ToString(dateTimeFormat);
            CMDProcess cmdProcess = new CMDProcess(new string[]
            {
                    $"cd /d {url}" +
                    $"hg --config \"extensions.shelve = \" shelve --name {shelfName} {debugArg} {tracebackArg}" +
                    $"hg pull {fullURL} {debugArg} {tracebackArg}" +
                    $"hg update -m {debugArg} {tracebackArg}" +
                    $"hg --config \"extensions.shelve = \" unshelve --name {shelfName} --tool :other {debugArg} {tracebackArg}"
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
