using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Kallithea_Klone.Other_Classes;

namespace Kallithea_Klone.States
{
    public abstract class TemplateState : IState
    {
        //  Variables
        //  =========
        
        protected readonly MainWindow mainWindow;

        //  Properties
        //  ==========

        public abstract string Verb { get; }

        //  Constructors
        //  ============

        public TemplateState()
        {
            mainWindow = MainWindow.singleton;
        }

        //  Abstract State Methods
        //  ======================

        public abstract void OnLoad();

        public abstract void OnLoaded();
        
        public abstract Task OnMainActionAsync(List<string> urls);

        public abstract void OnReload();

        public abstract void OnSearch();

        public abstract void OnSearchTermChanged();

        //  Implemented State Methods
        //  =========================


        /// <exception cref="System.Security.SecurityException">Ignore.</exception>
        /// <exception cref="InvalidCastException">Ignore.</exception>
        public virtual void OnLoseFocus()
        {
            if (Application.Current.Windows.Cast<Window>().Count(w => w.Focusable) == 1)
                Environment.Exit(0);
        }

        public virtual void OnSettings()
        {
            MainWindow.OpenSettings();
        }

        //  Other Methods
        //  =============

        /// <summary>
        /// Takes a folder location of a repository on disk and returns its default remote location
        /// </summary>
        /// <param name="repoLocation">The folder location of the repository on disk</param>
        /// <returns>The repositories default remote location</returns>
        /// <exception cref="Kallithea_Klone.MainActionException"></exception>
        protected string GetDefaultRemotePath(string repoLocation)
        {
            try
            {
                string hgrcFileLocation = Path.Combine(repoLocation, ".hg", "hgrc");

                if (!File.Exists(hgrcFileLocation))
                {
                    string folderName = Path.GetDirectoryName(repoLocation);
                    throw new FileNotFoundException($"The hgrc file for \"{folderName}\" could not be found!");
                }

                IniFile hgrcFile = new IniFile(hgrcFileLocation);

                if (!hgrcFile.KeyExists("default", "paths"))
                {
                    string folderName = Path.GetDirectoryName(repoLocation);
                    throw new KeyNotFoundException($"The default property could not be found in the .hg/hgrc file under \"[paths]\" for the folder \"{folderName}\"");
                }

                return hgrcFile.Read("default", "paths");
            }
            catch (Exception e) when (e is FileNotFoundException || e is KeyNotFoundException)
            {
                throw new MainActionException(e.Message, e);
            }
            catch (Exception e)
            {
                throw new MainActionException("Unable to find the default remote location in the hmrc file", e);
            }
        }

        /// <summary>
        /// Reports any error messages given by a CMDProcess while it ran
        /// </summary>
        /// <param name="cmdProcess">The CMDProcess to have its errors reported</param>
        /// <param name="verb">A verb describing what the curent state does</param>
        /// <exception cref="Kallithea_Klone.MainActionException"></exception>
        protected async Task ReportErrorsAsync(CMDProcess cmdProcess)
        {
            try
            {
                string errorMessages = await cmdProcess.GetErrorOutAsync();

                if (errorMessages.Length > 0)
                {
                    throw new MainActionException($"Finished with the exit code: {cmdProcess.ExitCode}\n\n" +
                        $"And the error messages: {errorMessages}");
                }
            }
            catch (Exception e)
            {
                throw new MainActionException($"Unable to read the process used for {Verb}. This means Kallithea" +
                    " Klone is unable to tell if it was successful or not.", e);
            }
        }
    }
}
