using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Kallithea_Klone.Other_Classes;

namespace Kallithea_Klone.States
{
    public abstract class TemplateState : IState
    {
        //  Properties
        //  ==========

        public abstract string Verb { get; }
        public string RunLocation { get; private set; }

        //  Constructors
        //  ============

        public TemplateState(string runLocation)
        {
            RunLocation = runLocation;
        }

        //  Abstract State Methods
        //  ======================

        /// <exception cref="InvalidOperationException"></exception>
        public abstract ICollection<Control> OnLoadRepositories();

        public abstract MainWindowStartProperties OnLoaded();
        
        public abstract Task OnMainActionAsync(List<string> urls);

        public abstract void OnReload();

        public abstract ICollection<Control> OnSearch(string searchTerm);

        public abstract ICollection<Control> OnSearchCleared(string searchTerm);

        //  Implemented State Methods
        //  =========================

        /// <exception cref="System.Security.SecurityException">Ignore.</exception>
        /// <exception cref="InvalidCastException">Ignore.</exception>
        public virtual void OnLoseFocus(bool completingMainAction)
        {
            if (!completingMainAction && Application.Current.Windows.Cast<Window>().Count(w => w.Focusable) == 1)
                Environment.Exit(0);
        }

        public virtual void OnSettings()
        {
            MainWindow.OpenSettings();
        }

        //  Other Methods
        //  =============

        /// <summary>
        /// Takes a folder location of a repository on disk and returns its default remote location
        /// </summary>
        /// <param name="repoLocation">The folder location of the repository on disk</param>
        /// <returns>The repositories default remote location</returns>
        /// <exception cref="Kallithea_Klone.MainActionException"></exception>
        protected string GetDefaultRemotePath(string repoLocation)
        {
            try
            {
                string hgrcFileLocation = Path.Combine(repoLocation, ".hg", "hgrc");

                if (!File.Exists(hgrcFileLocation))
                {
                    string folderName = Path.GetDirectoryName(repoLocation);
                    throw new FileNotFoundException($"The hgrc file for \"{folderName}\" could not be found!");
                }

                IniFile hgrcFile = new IniFile(hgrcFileLocation);

                if (!hgrcFile.KeyExists("default", "paths"))
                {
                    string folderName = Path.GetDirectoryName(repoLocation);
                    throw new KeyNotFoundException($"The default property could not be found in the .hg/hgrc file under \"[paths]\" for the folder \"{folderName}\"");
                }

                return hgrcFile.Read("default", "paths");
            }
            catch (Exception e) when (e is FileNotFoundException || e is KeyNotFoundException)
            {
                throw new MainActionException(e.Message, e);
            }
            catch (Exception e)
            {
                throw new MainActionException("Unable to find the default remote location in the hmrc file", e);
            }
        }

        /// <summary>
        /// Reports any error messages given by a CMDProcess while it ran
        /// </summary>
        /// <param name="cmdProcess">The CMDProcess to have its errors reported</param>
        /// <param name="verb">A verb describing what the curent state does</param>
        /// <exception cref="Kallithea_Klone.MainActionException"></exception>
        protected async Task ReportErrorsAsync(CMDProcess cmdProcess)
        {
            try
            {
                string errorMessages = await cmdProcess.GetErrorOutAsync();

                if (errorMessages.Length > 0)
                {
                    throw new MainActionException($"Finished with the exit code: {cmdProcess.ExitCode}\n\n" +
                        $"And the error messages: {errorMessages}");
                }
            }
            catch (Exception e) when (!(e is MainActionException))
            {
                throw new MainActionException($"Unable to read the process used for {Verb}. This means Kallithea" +
                    " Klone is unable to tell if it was successful or not.", e);
            }
        }
        
        public ICollection<Control> LoadRepositoryTree(ICollection<string> repositories)
        {
            Location baseLocation = new Location("Base Location");

            foreach (string location in repositories)
            {
                Location current = baseLocation;

                foreach (string part in location.Split('/'))
                {
                    current = GetOrCreateChild(current, part);
                }
            }

            SortChildren(baseLocation);

            ICollection<Control> results = new List<Control>();
            foreach (Location location in baseLocation.InnerLocations)
            {
                results.Add(CreateTreeControl(location));
            }

            return results;
        }

        public ICollection<Control> LoadRepositoryList(List<string> repositories)
        {
            ICollection<Control> results = new List<Control>();

            foreach (string location in repositories)
            {
                results.Add(CreateCheckBox(Path.GetFileName(location), location));
            }

            return results;
        }

        private void SortChildren(Location location)
        {
            foreach (Location subLocation in location.InnerLocations)
            {
                SortChildren(subLocation);
            }

            location.InnerLocations = location.InnerLocations.OrderBy(l => l.InnerLocations.Count == 0 ? 1 : 0).ThenBy(l => l.Name).ToList();
        }

        private Location GetOrCreateChild(Location current, string location)
        {
            if (current.InnerLocations.Count(l => l.Name == location) > 0)
            {
                return current.InnerLocations.FirstOrDefault(l => l.Name == location);
            }
            else
            {
                Location inner = new Location
                {
                    Name = location
                };
                current.InnerLocations.Add(inner);
                return inner;
            }
        }
        
        private Control CreateTreeControl(Location location, TreeViewItem parent = null)
        {
            if (location.InnerLocations.Count == 0)
            {
                string newTag = (parent == null) ? location.Name : parent.Tag + "/" + location.Name;
                return CreateCheckBox(location.Name, newTag);
            }
            else
            {
                return CreateTreeViewItem(location, parent);
            }
        }
        
        private CheckBox CreateCheckBox(string content, string tag)
        {
            CheckBox newCheckbox = new CheckBox
            {
                Content = content,
                VerticalContentAlignment = VerticalAlignment.Center,
                Tag = tag,
                IsChecked = MainWindow.CheckedURLs.Contains(tag)
            };
            newCheckbox.Checked += MainWindow.singleton.NewItem_Checked;
            newCheckbox.Unchecked += MainWindow.singleton.NewItem_Unchecked;

            return newCheckbox;
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        private Control CreateTreeViewItem(Location location, TreeViewItem parent)
        {
            TreeViewItem newTreeItem = new TreeViewItem
            {
                Header = location.Name,
                Tag = parent == null ? location.Name : parent.Tag + "/" + location.Name
            };

            foreach (Location subLocation in location.InnerLocations)
            {
                newTreeItem.Items.Add(CreateTreeControl(subLocation, newTreeItem));
            }

            return newTreeItem;
        }
    }
}
