using Kallithea_Klone.Other_Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kallithea_Klone.States
{
    public abstract class LocalEditorState : ActionState
    {
        //  Constructors
        //  ============

        public LocalEditorState(string runLocation) : base(runLocation)
        {

        }

        //  Implemented State Methods
        //  =========================

        /// <exception cref="InvalidOperationException"></exception>
        public override ICollection<string> OnLoadRepositories()
        {
            return GetLocalRepositories(RunLocation);
        }

        /// <exception cref="NotImplementedException">Ignore.</exception>
        public override Task<ICollection<string>> OnReloadAsync()
        {
            throw new NotImplementedException("Invalid Button Press!");
        }

        //  Other Methods
        //  =============

        private string[] GetLocalRepositories(string runLocation)
        {
            ICollection<string> foundRepositories = new List<string>();

            try
            {
                if (IsRepo(runLocation))
                {
                    return new string[] { runLocation };
                }

                foreach (string folder in Directory.GetDirectories(runLocation))
                {
                    if (IsRepo(folder))
                    {
                        foundRepositories.Add(folder);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Unable to read repositories from disk!\nPlease close all programs using that location and re-open Kallithea Klone",
                    "Error reading disk!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return foundRepositories.ToArray();
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
    }
}
