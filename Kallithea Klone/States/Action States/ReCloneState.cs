﻿using System;
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

        public override async Task OnMainActionAsync(ICollection<RepositoryData> locations)
        {
            foreach (RepositoryData location in locations)
            {
                try
                {
                    await ReClone(location);
                }
                catch (MainActionException e)
                {
                    MessageBox.Show($"Error {Verb} {location.Name}:\n" + e.Message, $"Error {Verb} {location.Name}", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        //  State Methods
        //  =============

        /// <exception cref="Kallithea_Klone.MainActionException"></exception>
        private async Task ReClone(RepositoryData location)
        {
            string remotePath = GetDefaultRemotePath(location.URL);
            Uri uri = new Uri(remotePath);

            string fullURL = $"{uri.Scheme}://{HttpUtility.UrlEncode(AccountSettings.Username)}:{HttpUtility.UrlEncode(AccountSettings.Password)}@{uri.Host}{uri.PathAndQuery}";

            try
            {
                ClearOutRepository(location.Name);
            }
            catch (Exception e)
            {
                throw new MainActionException($"Unable to properly delete the original repository, it is now probably half deleted.", e);
            }

            CMDProcess cmdProcess = new CMDProcess("RE-CLONE", location.Name, new string[]
            {
                    $"cd /d \"{location.URL}\"",
                    $"hg init {debugArg}",
                    $"hg pull {fullURL} {debugArg}",
                    $"hg update {debugArg}"
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
            SetDefaultLocation(location.URL, passwordSafeURL);
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