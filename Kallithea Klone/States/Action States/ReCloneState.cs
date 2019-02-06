using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Web;
using Kallithea_Klone.Other_Classes;
using Kallithea_Klone.Account_Settings;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Kallithea_Klone.States
{
    class ReCloneState : LocalEditorState
    {
        //  Properties
        //  ==========

        public override string Verb => "re-cloning";

        //  Constructors
        //  ============

        public ReCloneState(string runLocation) : base(runLocation)
        {

        }

        //  State Pattern
        //  =============

        public override MainWindowStartProperties OnLoaded()
        {
            return new MainWindowStartProperties
            {
                Title = "Kallithea Reclone",
                MainActionContent = "ReClone",
                ReloadVisibility = Visibility.Hidden
            };
        }

        public override async Task OnMainActionAsync(List<string> urls)
        {
            foreach (string url in urls)
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

        //  State Methods
        //  =============

        /// <exception cref="Kallithea_Klone.MainActionException"></exception>
        private async Task ReClone(string url)
        {
            string remotePath = GetDefaultRemotePath(url);
            Uri uri = new Uri(remotePath);

            string fullURL = $"{uri.Scheme}://{HttpUtility.UrlEncode(AccountSettings.Username)}:{HttpUtility.UrlEncode(AccountSettings.Password)}@{uri.Host}{uri.PathAndQuery}";

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

            cmdProcess.ReportErrorsAsync(Verb);

            string passwordSafeURL = $"{uri.Scheme}://{HttpUtility.UrlEncode(AccountSettings.Username)}@{uri.Host}{uri.PathAndQuery}";
            SetDefaultLocation(url, passwordSafeURL);
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