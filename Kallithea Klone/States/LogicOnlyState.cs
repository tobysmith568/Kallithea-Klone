using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows.Controls;
using Kallithea_Klone.Other_Classes;

namespace Kallithea_Klone.States
{
    public abstract class LogicOnlyState : IState
    {
        //  Abstract State Methods
        //  ======================

        public abstract void InitialActions(string[] args);

        //  State Methods
        //  =============

        public string Verb => throw new InvalidOperationException();

        public string RunLocation => throw new InvalidOperationException();

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        public MainWindowStartProperties OnLoaded()
        {
            throw new InvalidOperationException();
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        public ICollection<Control> OnLoadRepositories()
        {
            throw new InvalidOperationException();
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        public void OnLoseFocus(bool completingMainAction)
        {
            throw new InvalidOperationException();
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        public Task OnMainActionAsync(List<Repo> urls)
        {
            throw new InvalidOperationException();
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        public Task<ICollection<Control>> OnReloadAsync()
        {
            throw new InvalidOperationException();
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        public ICollection<Control> OnSearch(string searchTerm)
        {
            throw new InvalidOperationException();
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        public ICollection<Control> OnSearchCleared(string searchTerm)
        {
            throw new InvalidOperationException();
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        public void OnSettings()
        {
            throw new InvalidOperationException();
        }

        //  Other Methods
        //  =============

        /// <exception cref="System.Security.SecurityException">Ignore.</exception>
        protected static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        /// <exception cref="FileNotFoundException">Ignore.</exception>
        /// <exception cref="System.ComponentModel.Win32Exception">Ignore.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Ignore.</exception>
        protected void RestartAsAdmin(string arguments)
        {
            string exeName = Process.GetCurrentProcess().MainModule.FileName;

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(exeName)
                {
                    Verb = "runas",
                    Arguments = arguments
                };
                Process.Start(startInfo);
            }
            catch
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(exeName)
                {
                    Arguments = arguments
                };
                Process.Start(startInfo);
            }
            App.Current.Shutdown();
        }
    }
}
