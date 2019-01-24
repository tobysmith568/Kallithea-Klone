using Kallithea_Klone.Other_Classes;
using Kallithea_Klone.States;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.Windows;
using System.Windows.Forms;
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

            if (e.Args.Length > 0)
            {
                switch (UppercaseFirst(e.Args[0]))
                {
                    case nameof(RunTypes.Clone):
                        if (e.Args.Length >= 2)
                            Open(RunTypes.Clone, e.Args[1]);
                        else
                            goto default;
                        break;
                    case nameof(RunTypes.LocalRevert):
                        if (e.Args.Length >= 2)
                            Open(RunTypes.LocalRevert, e.Args[1]);
                        else
                            goto default;
                        break;
                    case nameof(RunTypes.Reclone):
                        if (e.Args.Length >= 2)
                            Open(RunTypes.Reclone, e.Args[1]);
                        else
                            goto default;
                        break;
                    case nameof(RunTypes.Update):
                        if (e.Args.Length >= 2)
                            Open(RunTypes.Update, e.Args[1]);
                        else
                            goto default;
                        break;
                    case nameof(RunTypes.Settings):
                        Settings();
                        break;
                    case nameof(RunTypes.Setup):
                        if (e.Args.Length >= 2)
                            Setup(e.Args[1]);
                        else
                            Setup();
                        goto default;
                    case nameof(RunTypes.Uninstall):
                        if (e.Args.Length >= 2)
                            Uninstall(e.Args[1]);
                        else
                            Uninstall();
                        goto default;
                    default:
                        Environment.Exit(1);
                        break;
                }
            }
            else
            {
                string folder;

                if (CommonFileDialog.IsPlatformSupported)
                {
                    folder = SelectWinVistaFolder();
                }
                else
                {
                    folder = SelectWinXPFolder();
                }

                Open(RunTypes.Clone, folder);
            }
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

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        /// <exception cref="Exception">Ignore.</exception>
        private void Open(RunTypes runType, string ranFrom)
        {
            MainWindow window = new MainWindow(runType, ranFrom);

            int windowHeight = (int)window.Height;
            int windowWidth = (int)window.Width;

            window.Left = Cursor.Position.X - (windowWidth / 2);
            window.Top = Cursor.Position.Y - (windowHeight / 2);

            int screenHeight = Screen.FromPoint(Cursor.Position).Bounds.Height;
            int screenWidth = Screen.FromPoint(Cursor.Position).Bounds.Width;

            while (window.Top + windowHeight + 5 > screenHeight)
                window.Top -= 1;

            while (window.Top - 5 < 0)
                window.Top += 1;

            while (window.Left + windowWidth + 5 > screenWidth)
                window.Left -= 1;

            while (window.Left - 5 < 0)
                window.Left += 1;

            window.Show();
        }

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
                ContextMenuImplementations.CreateStandard();

                if (AccountSettings.AdvancedOptions)
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

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        private void Settings()
        {
            Settings window = new Settings();

            int windowHeight = (int)window.Height;
            int windowWidth = (int)window.Width;

            window.Left = Cursor.Position.X - (windowWidth / 2);
            window.Top = Cursor.Position.Y - (windowHeight / 2);

            int screenHeight = Screen.FromPoint(Cursor.Position).Bounds.Height;
            int screenWidth = Screen.FromPoint(Cursor.Position).Bounds.Width;

            while (window.Top + windowHeight + 5 > screenHeight)
                window.Top -= 1;

            while (window.Top - 5 < 0)
                window.Top += 1;

            while (window.Left + windowWidth + 5 > screenWidth)
                window.Left -= 1;

            while (window.Left - 5 < 0)
                window.Left += 1;

            window.Show();
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
            var exeName = Process.GetCurrentProcess().MainModule.FileName;

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

        /// <exception cref="System.Security.SecurityException">Ignore.</exception>
        private string SelectWinXPFolder()
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog
            {
                Description = "Select a target folder"
            })
            {
                DialogResult result = folderBrowserDialog.ShowDialog();

                switch (result)
                {
                    case DialogResult.OK:
                    case DialogResult.Yes:
                        break;

                    default:
                        Environment.Exit(1);
                        break;
                }

                return folderBrowserDialog.SelectedPath;
            }
        }

        /// <exception cref="System.Security.SecurityException">Ignore.</exception>
        private string SelectWinVistaFolder()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                Title = "Select a target folder",
                IsFolderPicker = true,
                DefaultDirectory = @"C:\",
                AllowNonFileSystemItems = false,
                EnsurePathExists = true,
                Multiselect = false,
                NavigateToShortcut = true
            })
            {
                CommonFileDialogResult result = dialog.ShowDialog();

                switch (result)
                {
                    case CommonFileDialogResult.Ok:
                        break;

                    default:
                        Environment.Exit(1);
                        break;
                }

                return dialog.FileName;
            }
        }
    }
}