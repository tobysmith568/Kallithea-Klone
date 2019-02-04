﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kallithea_Klone.States
{
    public abstract class LocalEditorState : TemplateState
    {
        //  Constructors
        //  ============

        public LocalEditorState(string runLocation) : base(runLocation)
        {

        }

        //  Implemented State Methods
        //  =========================

        /// <exception cref="InvalidOperationException"></exception>
        public override ICollection<Control> OnLoadRepositories()
        {
            return LoadRepositoryTree(GetLocalRepositories(RunLocation));
        }

        public override ICollection<Control> OnSearch(string searchTerm)
        {
            IEnumerable<string> repositories = GetLocalRepositories(RunLocation);
            foreach (string term in searchTerm.ToLower().Split(' '))
            {
                repositories = repositories.Where(r => r.ToLower().Contains(term));
            }
            return LoadRepositoryList(repositories.ToList());
        }

        public override ICollection<Control> OnSearchCleared(string searchTerm)
        {
            return LoadRepositoryTree(GetLocalRepositories(RunLocation));
        }

        /// <exception cref="NotImplementedException">Ignore.</exception>
        public override void OnReload()
        {
            throw new NotImplementedException("Invalid Button Press!");
        }

        //  Other Methods
        //  =============

        protected ICollection<string> GetLocalRepositories(string runLocation)
        {
            ICollection<string> foundRepositories = new List<string>();

            try
            {
                if (IsRepo(runLocation))
                {
                    return new List<string> { runLocation };
                }

                foreach (string folder in Directory.GetDirectories(runLocation))
                {
                    string folderName = Path.GetFileName(folder);

                    if (IsRepo(folderName))
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

            return foundRepositories;
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
