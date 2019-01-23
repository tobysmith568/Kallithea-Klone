using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.Web;
using System.Diagnostics;
using Kallithea_Klone.Other_Classes;

namespace Kallithea_Klone.States
{
    class ReCloneState : TemplateState
    {
        //  Variables
        //  =========

        private int reCloningCount;
        private int reClonedCount = 0;
        private List<string> errorCodes = new List<string>();

        //  Constructors
        //  ============

        public ReCloneState() : base()
        {

        }

        //  Events
        //  ======

        private void Process_Exited(object sender, EventArgs e)
        {
            Process sendingProcess = (Process)sender;
            int exitCode = sendingProcess.ExitCode;

            if (exitCode != 0)
            {
                errorCodes.Add(exitCode.ToString());
            }
            else
            {
                string[] arguments = sendingProcess.StartInfo.Arguments.Split('|');

                if (arguments.Length < 2)
                {
                    throw new ArgumentOutOfRangeException("Cannot find local result from cloning");
                }

                string uriPairArg = arguments[arguments.Length - 1];

                URIPair uriPair = JsonConvert.DeserializeObject<URIPair>(uriPairArg);

                try
                {
                    IniFile hgrc = new IniFile(Path.Combine(uriPair.Local, ".hg", "hgrc"));
                    hgrc.Write("default", uriPair.Remote, "paths");
                }
                catch (PathTooLongException)
                {
                    MessageBox.Show($"Unable to read the hgrc file for the repository at {uriPair.Local} because the file path is too long.\n" +
                        $"This will cause problems later with other Kallithea Klone actions.");
                }
                catch (Exception ex) when (ex is System.Security.SecurityException || ex is UnauthorizedAccessException)
                {
                    MessageBox.Show($"Unable to read the hgrc file for the repository at {uriPair.Local}.\n" +
                        $"This will cause problems later with other Kallithea Klone actions.");
                }


            }

            reClonedCount++;
            CheckClonedCount();
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
            mainWindow.BtnMainAction.Content = "Reclone";
            mainWindow.LblTitle.Content = "Kallithea Reclone";
            mainWindow.BtnReload.Visibility = Visibility.Hidden;
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        /// <exception cref="System.ComponentModel.Win32Exception">Ignore.</exception>
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        public override void OnMainAction()
        {
            if (!SettingsNotEmpty())
            {
                MessageBoxResult result = MessageBox.Show("It looks like you have not properly set up your settings.\n" +
                     "Would you like to open them now?", "Empty settings!", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);

                switch (result)
                {
                    case MessageBoxResult.OK:
                        mainWindow.OpenSettings();
                        break;
                    default:
                        break;
                }
                return;
            }

            mainWindow.DisableAll();

            reCloningCount = MainWindow.CheckedURLs.Count;
            foreach (string url in MainWindow.CheckedURLs)
            {
                string remotePath;
                try
                {
                    remotePath = GetDefaultRemotePath(url);
                }
                catch (Exception)
                {
                    MessageBox.Show($"Unable to re-clone the repository at {url}.");
                    reCloningCount--;
                    CheckClonedCount();
                    continue;
                }

                Uri uri = new Uri(remotePath);
                string fullURL = $"{uri.Scheme}://{HttpUtility.UrlEncode(AccountSettings.Username)}:{HttpUtility.UrlEncode(AccountSettings.Password)}@{uri.Host}{uri.PathAndQuery}";

                try
                {
                    foreach (string folder in Directory.GetDirectories(url))
                    {
                        string last = folder.Split('\\').Last();
                        if (last == ".hg" || last.First() != '.')
                            Directory.Delete(folder, true);
                    }

                    foreach (string file in Directory.GetFiles(url))
                    {
                        File.Delete(file);
                    }
                }
                catch
                {
                    MessageBox.Show($"Error: Failed to properly delete \"{url.Split('\\').Last()}\"\nThis repository is now probably half deleted.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                    continue;
                }

                string urlPair = JsonConvert.SerializeObject(new URIPair(url, fullURL));

                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Normal,
                    FileName = CmdExe,
                    Arguments = $"/C cd /d \"{url}\"" +
                                  $"&hg init" +
                                  $"&hg pull {fullURL}" +
                                  $"&hg update" +
                                  $"&echo \"|{urlPair}"//Allows the process closed event to have the URLs via the process's starting arguments
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

        /// <exception cref="System.Security.SecurityException">Ignore.</exception>
        private void CheckClonedCount()
        {
            if (reClonedCount == reCloningCount)
            {
                if (errorCodes.Count > 0)
                    MessageBox.Show("Finshed, but with the following mercurial exit codes:\n" + string.Join("\n", errorCodes), "Errors", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                Environment.Exit(0);
            }
        }
    }
}
