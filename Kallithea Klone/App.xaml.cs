using Kallithea_Klone.Account_Settings;
using Kallithea_Klone.Other_Classes;
using Kallithea_Klone.States;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows;
using System.Windows.Forms;
using System.Linq;
using System.Windows.Threading;

namespace Kallithea_Klone
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        //  Events
        //  ======

        /// <exception cref="System.Security.SecurityException">Ignore.</exception>
        void App_Startup(object sender, StartupEventArgs e)
        {
            if (AccountSettings.JustInstalled)
            {
                AccountSettings.Upgrade();
                AccountSettings.JustInstalled = false;
            }

            if (e.Args.Length == 0)
            {
                RunTypes.Clone.Open(new string[0]);
                return;
            }

            if (Enum.TryParse(UppercaseFirst(e.Args[0]), out RunTypes runType))
            {
                runType.Open(e.Args.Skip(1).ToArray());
                return;
            }

            System.Windows.MessageBox.Show($"Invalid initial argument [{e.Args[0]}] given!",
                "Command Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <exception cref="System.Security.SecurityException"></exception>
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            System.Windows.MessageBox.Show("An unexpected exception has occurred. Shutting down the application:\n" + args.Exception);

            // Prevent default unhandled exception processing
            args.Handled = true;

            Environment.Exit(0);
        }

        //  Methods
        //  =======

        private void Setup(string attempt = "0")
        {
            if (!IsAdministrator())
            {
                if (attempt == "0")
                    RestartAsAdmin("Setup 1");
                else
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show("This installer needs administrator permissions in order to edit your Windows Explorer Menus.\n" +
                        "This is essential for the primary functionality. Press OK to try again, or Cancel to install without this essential functionality", "Kallithea Klone needs administrator permissions!", MessageBoxButton.OKCancel, MessageBoxImage.Error);

                    switch (result)
                    {
                        case MessageBoxResult.OK:
                            RestartAsAdmin("Setup 1");
                            break;
                        default:
                            break;
                    }
                }
                return;
            }

            try
            {
                ContextMenuImplementations.RemoveAll();
                ContextMenuImplementations.CreateStandard();
                ContextMenuImplementations.CreateAdvanced();
            }
            catch
            {
                System.Windows.MessageBox.Show("Kallithea Klone was unable to add all of the Windows Explorer context menu items it uses.\n" +
                    "Please re-run the installer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Uninstall(string attempt = "0")
        {
            if (!IsAdministrator())
            {
                if (attempt == "0")
                    RestartAsAdmin("Uninstall 1");
                else
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show("This uninstaller need administrator permissions in order to edit your Windows Explorer Menus.\n" +
                        "This is essential to remove unused menu items. Press OK to try again or Cancel to leave the items after the uninstall.", "Kallithea Klone needs administrator permissions!", MessageBoxButton.OKCancel, MessageBoxImage.Error);

                    switch (result)
                    {
                        case MessageBoxResult.OK:
                            RestartAsAdmin("Uninstall 1");
                            break;
                        default:
                            break;
                    }
                }
                return;
            }

            try
            {
                ContextMenuImplementations.RemoveAll();
            }
            catch
            {
                System.Windows.MessageBox.Show("Kallithea Klone was unable to remove all Windows Explorer context menu items.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            AccountSettings.Reset();
            AccountSettings.Save();

            try
            {
                string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Kallithea Klone");
                foreach (string file in Directory.GetFiles(appDataFolder))
                {
                    File.Delete(file);
                }
                Directory.Delete(appDataFolder);
            }
            catch
            {
                //If app data cannot be removed then it will have to stay
            }
        }

        /// <exception cref="System.Security.SecurityException">Ignore.</exception>
        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            return char.ToUpper(s[0]) + s.Substring(1);
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        /// <exception cref="FileNotFoundException">Ignore.</exception>
        /// <exception cref="System.ComponentModel.Win32Exception">Ignore.</exception>
        private void RestartAsAdmin(string arguments)
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
            Current.Shutdown();
        }
    }
}