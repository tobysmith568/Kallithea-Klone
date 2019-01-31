using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Web;
using Kallithea_Klone.Other_Classes;
using Kallithea_Klone.Account_Settings;
using System.Threading.Tasks;

namespace Kallithea_Klone.States
{
    class ReCloneState : TemplateState
    {
        //  Properties
        //  ==========

        public override string Verb => "re-cloning";

        //  Constructors
        //  ============

        public ReCloneState() : base()
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
            mainWindow.BtnMainAction.Content = "Reclone";
            mainWindow.LblTitle.Content = "Kallithea Reclone";
            mainWindow.BtnReload.Visibility = Visibility.Hidden;
        }

        public override async Task OnMainActionAsync()
        {
            foreach (string url in MainWindow.CheckedURLs)
            {
                try
                {
                    await ReClone(url);
                }
                catch (MainActionException e)
                {
                    MessageBox.Show($"Error {Verb} {Path.GetFileName(url)}:\n" + e.Message, $"Error {Verb} {Path.GetFileName(url)}", MessageBoxButton.OK, MessageBoxImage.Error);
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

        /// <exception cref="Kallithea_Klone.MainActionException"></exception>
        private async Task ReClone(string url)
        {
            string remotePath = GetDefaultRemotePath(url);
            Uri uri = new Uri(remotePath);

            string fullURL = $"{uri.Scheme}://{HttpUtility.UrlEncode(AccountSettings.Username)}:{HttpUtility.UrlEncode(AccountSettings.Password)}@{uri.Host}{uri.PathAndQuery}";
            string passwordSafeURL = $"{uri.Scheme}://{HttpUtility.UrlEncode(AccountSettings.Username)}@{uri.Host}{uri.PathAndQuery}";

            try
            {
                ClearOutRepository(url);
            }
            catch (Exception e)
            {
                throw new MainActionException($"Unable to properly delete the original repository, it is now probably half deleted.", e);
            }

            CMDProcess cmdProcess = new CMDProcess(new string[]
            {
                    $"cd /d \"{url}\"",
                    $"hg init",
                    $"hg pull {fullURL}",
                    $"hg update"
            });

            try
            {
                await cmdProcess.Run();
            }
            catch (Exception e)
            {
                throw new MainActionException("Unable to start the necessary command window process", e);
            }

            await ReportErrorsAsync(cmdProcess);

            SetDefaultLocation(url, passwordSafeURL);
        }

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

        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="PathTooLongException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private static void ClearOutRepository(string url)
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

        private static void SetDefaultLocation(string url, string passwordSafeURL)
        {
            try
            {
                IniFile hgrc = new IniFile(Path.Combine(url, ".hg", "hgrc"));
                hgrc.Write("default", passwordSafeURL, "paths");
            }
            catch (PathTooLongException)
            {
                MessageBox.Show($"Unable to read the hgrc file for the repository at {url} because the file path is too long.\n" +
                    $"This will cause problems later with other Kallithea Klone actions.");
            }
            catch (Exception ex) when (ex is System.Security.SecurityException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show($"Unable to read the hgrc file for the repository at {url}.\n" +
                    $"This will cause problems later with other Kallithea Klone actions.");
            }
        }
    }
}
