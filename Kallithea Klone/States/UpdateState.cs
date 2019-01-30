using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Web;
using System.Diagnostics;
using Kallithea_Klone.Account_Settings;
using System.Threading.Tasks;
using Kallithea_Klone.Other_Classes;

namespace Kallithea_Klone.States
{
    class UpdateState : TemplateState
    {
        //  Variables
        //  =========

        private const string dateTimeFormat = "ddMMyyHHmmss";

        //  Constructors
        //  ============

        public UpdateState() : base()
        {

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
            mainWindow.BtnMainAction.Content = "Update";
            mainWindow.LblTitle.Content = "Kallithea Update";
            mainWindow.BtnReload.Visibility = Visibility.Hidden;
        }

        public override async Task OnMainActionAsync()
        {
            foreach (string url in MainWindow.CheckedURLs)
            {
                string remotePath;
                try
                {
                    remotePath = GetDefaultRemotePath(url);
                }
                catch
                {
                    MessageBox.Show($"Unable to update the repository at {url} because Kallithea Klone couldn't find its default" +
                        $" remote location in its hmrc file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    continue;
                }

                Uri uri = new Uri(remotePath);

                string fullURL = $"{uri.Scheme}://{HttpUtility.UrlEncode(AccountSettings.Username)}:{HttpUtility.UrlEncode(AccountSettings.Password)}@{uri.Host}{uri.PathAndQuery}";
                string shelfName = DateTime.Now.ToString(dateTimeFormat);
                CMDProcess cmdProcess = new CMDProcess(new string[]
                {
                    $"cd /d {url}" +
                    $"hg --config \"extensions.shelve = \" shelve --name {shelfName}" +
                    $"hg pull {fullURL}" +
                    $"hg update -m" +
                    $"hg --config \"extensions.shelve = \" unshelve --name {shelfName} --tool :other"
                });

                try
                {
                    await cmdProcess.Run();
                }
                catch
                {
                    MessageBox.Show($"Unable to start the process needed to update {Path.GetFileName(url)}", "Error!",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }

                try
                {
                    string errorMessages = await cmdProcess.GetErrorOutAsync();

                    if (errorMessages.Length > 0)
                    {
                        string location = Path.GetFileName(url);
                        MessageBox.Show($"{location} finished with the exit code: {cmdProcess.ExitCode}\n\n" +
                            $"And the error messages: {errorMessages}",
                            $"Exit code {cmdProcess.ExitCode} while updating {location}!",
                            MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                        continue;
                    }
                }
                catch
                {
                    MessageBox.Show($"Unable to read the process used to update {Path.GetFileName(url)}. This means Kallithea" +
                        $"Klone is unable to tell if it was successful or not.", "Error!",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <exception cref="NotImplementedException">Ignore.</exception>
        public override void OnReload()
        {
            throw new NotImplementedException("Invalid Button Press!");
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
