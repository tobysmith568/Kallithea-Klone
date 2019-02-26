using System;
using System.IO;
using System.Windows;
using System.Threading.Tasks;
using Kallithea_Klone.Other_Classes;
using System.Collections.Generic;

namespace Kallithea_Klone.States
{
    class LocalRevertState : LocalEditorState
    {
        //  Properties
        //  ==========

        public override string Verb => "reverting";

        //  Constructors
        //  ============

        public LocalRevertState(string runLocation) : base(runLocation)
        {

        }

        //  State Methods
        //  =============

        public override MainWindowStartProperties OnLoaded()
        {
            return new MainWindowStartProperties
            {
                Title = "Kallithea Revert",
                MainActionContent = "Local Revert",
                ReloadVisibility = Visibility.Hidden
            };
        }
        
        public override async Task OnMainActionAsync(List<Repo> repos)
        {
            foreach (Repo repo in repos)
            {
                try
                {
                    await Revert(repo);
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
        private async Task Revert(Repo repo)
        {
            CMDProcess cmdProcess = new CMDProcess("REVERT", repo.Name, new string[]
            {
                    $"cd /d {repo.URL}",
                    $"hg revert --all {debugArg}",
                    $"hg --config \"extensions.purge = \" purge --all {debugArg}"
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
