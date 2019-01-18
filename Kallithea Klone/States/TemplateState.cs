using System;
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

        protected string GetDefaultPath(string repoLocation)
        {
            string hgrcFile = Path.Combine(repoLocation, ".hg\\hgrc");

            if (!File.Exists(hgrcFile))
            {
                MessageBox.Show($"Error: the hgrc file for \"{repoLocation.Split('\\').Last()}\" could not be found!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                return null;
            }

            IniFile hgrc = new IniFile(hgrcFile);

            if (!hgrc.KeyExists("default", "paths"))
            {
                MessageBox.Show($"Error: the default property \"{repoLocation.Split('\\').Last()}\" could not be found in it's .hg/hgrc file under \"[paths]\"!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                return null;
            }

            return hgrc.Read("default", "paths");
        }
    }
}
