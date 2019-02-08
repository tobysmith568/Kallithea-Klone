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
        
        public override async Task OnMainActionAsync(List<string> urls)
        {
            foreach (string url in urls)
            {
                try
                {
                    await Revert(url);
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
        private async Task Revert(string url)
        {
            CMDProcess cmdProcess = new CMDProcess(new string[]
            {
                    $"cd /d {url}",
                    $"hg revert --all {debugArg} {tracebackArg}",
                    $"hg --config \"extensions.purge = \" purge --all {debugArg} {tracebackArg}"
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
