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
    public abstract class ActionState : IState
    {
        //  Variables
        //  =========

        protected const string debugArg = "--debug";
        protected const string tracebackArg = "--traceback";

        //  Properties
        //  ==========

        public abstract string Verb { get; }
        public string RunLocation { get; private set; }

        //  Constructors
        //  ============

        public ActionState(string runLocation)
        {
            RunLocation = runLocation;
        }

        //  Abstract State Methods
        //  ======================

        /// <exception cref="InvalidOperationException"></exception>
        public abstract ICollection<Location> OnLoadRepositories();

        public abstract MainWindowStartProperties OnLoaded();
        
        public abstract Task OnMainActionAsync(List<Location> locations);

        public abstract Task<ICollection<Location>> OnReloadAsync();

        //  Implemented State Methods
        //  =========================

        public void InitialActions(string[] args)
        {
            //Nothing by default
        }

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
            catch (Exception e) when (!(e is MainActionException))
            {
                throw new MainActionException("Unable to find the default remote location in the hmrc file", e);
            }
        }

        protected ICollection<Location> CreateLocations(string[] urls)
        {
            List<Location> results = new List<Location>();

            foreach (string url in urls)
            {
                results.Add(new Location(url));
            }

            return results;
        }
    }
}
