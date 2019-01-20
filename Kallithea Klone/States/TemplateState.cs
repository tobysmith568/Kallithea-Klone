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
        protected MainWindow mainWindow;

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
            mainWindow.OpenSettings();
        }

        //  Other Methods
        //  =============

        protected bool ValidSettings()
        {
            return MainWindow.Host != ""
                && MainWindow.APIKey != ""
                && MainWindow.Username != ""
                && MainWindow.Password != "";
        }

        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="KeyNotFoundException"></exception>
        protected string GetDefaultPath(string repoLocation)
        {
            Uri repoUri = new Uri(repoLocation);
            string folderName = Path.GetDirectoryName(repoLocation);

            if (!repoUri.IsFile)
                throw new ArgumentException("The given repo location is not a valid file location.");

            string hgrcFile = Path.Combine(repoLocation, ".hg", "hgrc");

            if (!File.Exists(hgrcFile))
            {
                throw new FileNotFoundException($"The hgrc file for \"{folderName}\" could not be found!");
            }

            IniFile hgrc = new IniFile(hgrcFile);

            if (!hgrc.KeyExists("default", "paths"))
            {
                throw new KeyNotFoundException($"The default property could not be found in the .hg/hgrc file under \"[paths]\" for the folder \"{folderName}\"");
            }

            return hgrc.Read("default", "paths");
        }
    }
}
