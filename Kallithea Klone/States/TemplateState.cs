using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Kallithea_Klone.Other_Classes;

namespace Kallithea_Klone.States
{
    public abstract class TemplateState : IState
    {
        //  Variables
        //  =========

        protected const string CmdExe = "cmd.exe";
        protected readonly MainWindow mainWindow;

        //  Constructors
        //  ============

        public TemplateState()
        {
            mainWindow = MainWindow.singleton;
        }

        //  Abstract State Methods
        //  ======================

        public abstract void OnLoad();

        public abstract void OnMainAction();

        public abstract void OnReload();

        public abstract void OnSearch();

        public abstract void OnSearchTermChanged();

        //  Implemented State Methods
        //  =========================

        public virtual void OnLoaded()
        {

        }

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
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        protected string GetDefaultRemotePath(string repoLocation)
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
    }
}
