using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;

namespace Kallithea_Klone.States
{
    class LocalRevertState : TemplateState
    {
        //  Variables
        //  =========

        private int revertingCount;
        private int revertedCount = 0;
        private List<string> errorCodes = new List<string>();

        //  Constructors
        //  ============

        public LocalRevertState() : base()
        {

        }

        //  Events
        //  ======

        /// <exception cref="System.Security.SecurityException">Ignore.</exception>
        private void Process_Exited(object sender, EventArgs e)
        {
            if (((Process)sender).ExitCode != 0)
                errorCodes.Add(((Process)sender).ExitCode.ToString());
            revertedCount++;

            if (revertedCount == revertingCount)
            {
                if (errorCodes.Count > 0)
                    MessageBox.Show("Finshed, but with the following mercurial exit codes:\n" + String.Join("\n", errorCodes), "Errors", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                Environment.Exit(0);
            }
        }

        //  State Pattern
        //  =============

        public override void OnLoad()
        {
            try
            {
                LoadRepositories();
            }
            catch
            {
                MessageBox.Show("Unable to load repositories. Please close and re-open Kallithea Klone",
                    "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public override void OnLoaded()
        {
            mainWindow.BtnMainAction.Content = "Local Revert";
            mainWindow.LblTitle.Content = "Kallithea Revert";
            mainWindow.BtnReload.Visibility = Visibility.Hidden;
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        /// <exception cref="System.ComponentModel.Win32Exception">Ignore.</exception>
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        public override void OnMainAction()
        {
            mainWindow.DisableAll();
            Uri uri = new Uri(AccountSettings.Host);

            revertingCount = MainWindow.CheckedURLs.Count;
            foreach (string url in MainWindow.CheckedURLs)
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = CmdExe,
                    Arguments = $"/C cd /d {url}" +
                                  $"&hg revert --all" +
                                  $"&hg --config \"extensions.purge = \" purge --all"
                };
                process.StartInfo = startInfo;
                process.EnableRaisingEvents = true;
                process.Exited += Process_Exited;

                process.Start();
            }
        }

        /// <exception cref="Exception">Ignore.</exception>
        public override void OnReload()
        {
            throw new Exception("Invalid Button Press!");
        }

        public override void OnSearch()
        {
            if (mainWindow.TbxSearch.Text.Length != 0)
            {
                try
                {
                    LoadRepositories(mainWindow.TbxSearch.Text.Split(' '));
                }
                catch
                {
                    MessageBox.Show("Unable to load repositories. Please close and re-open Kallithea Klone",
                        "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public override void OnSearchTermChanged()
        {
            if (mainWindow.TbxSearch.Text.Length == 0)
            {
                try
                {
                    LoadRepositories();
                }
                catch
                {
                    MessageBox.Show("Unable to load repositories. Please close and re-open Kallithea Klone",
                        "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //  Other Methods
        //  =============

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        private void LoadRepositories(string[] searchTerms = null)
        {
            mainWindow.MainTree.Items.Clear();

            string name = mainWindow.runFrom.Split('\\').Last().ToLower();

            if (IsRepo(mainWindow.runFrom) && (searchTerms == null || searchTerms.Where(t => name.Contains(t.ToLower())).Count() > 0))
            {
                mainWindow.MainTree.Items.Add(CreateRepo(mainWindow.runFrom));
            }
            else foreach (string folder in Directory.GetDirectories(mainWindow.runFrom))
                {
                    name = folder.Split('\\').Last().ToLower();

                    if (IsRepo(folder) && (searchTerms == null || searchTerms.Where(t => name.Contains(t.ToLower())).Count() > 0))
                    {
                        mainWindow.MainTree.Items.Add(CreateRepo(folder));
                    }
                }

            mainWindow.SelectionUpdated();
        }

        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        private bool IsRepo(string path)
        {
            string[] innerFolders = Directory.GetDirectories(path);
            foreach (string folder in innerFolders)
            {
                Uri uri = new Uri(folder);

                if (!uri.IsFile)
                    continue;

                if (Path.GetFileName(uri.LocalPath) == ".hg")
                {
                    return true;
                }
            }
            return false;
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        private CheckBox CreateRepo(string location)
        {
            CheckBox newItem = new CheckBox
            {
                Content = location.Split('\\').Last(),
                Tag = location,
                VerticalContentAlignment = VerticalAlignment.Center,
                IsChecked = MainWindow.CheckedURLs.Contains(location)
            };
            newItem.Checked += mainWindow.NewItem_Checked;
            newItem.Unchecked += mainWindow.NewItem_Unchecked;

            return newItem;
        }
    }
}
