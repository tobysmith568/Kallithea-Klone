using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using static Kallithea_Klone.Properties.Settings;

namespace Kallithea_Klone
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        //  Events
        //  ======

        void App_Startup(object sender, StartupEventArgs e)
        {
            if (Default.JustInstalled)
            {
                Default.Upgrade();
                Default.JustInstalled = false;
                Default.Save();
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

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {

            System.Windows.MessageBox.Show("An unexpected exception has occurred. Shutting down the application:\n" + args.Exception);

            // Prevent default unhandled exception processing
            args.Handled = true;

            Environment.Exit(0);
        }

        //  Methods
        //  =======

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

            string location = System.Reflection.Assembly.GetExecutingAssembly().Location;

            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"Directory\Background\shell", true))
            {
                using (RegistryKey subKey = key.CreateSubKey("KallitheaKlone", RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    subKey.SetValue("MUIVerb", "Open Kallithea Klone here", RegistryValueKind.String);
                    subKey.SetValue("Icon", "\"" + location + "\"", RegistryValueKind.String);
                    using (RegistryKey commandKey = subKey.CreateSubKey("command", RegistryKeyPermissionCheck.ReadWriteSubTree))
                    {
                        commandKey.SetValue("", "\"" + location + "\" Clone \"%V\"");
                    }
                }

                using (RegistryKey subKey = key.CreateSubKey("KallitheaKlone_Other", RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    subKey.SetValue("MUIVerb", "Other Kallithea Klone Options", RegistryValueKind.String);
                    subKey.SetValue("SubCommands", "KallitheaKlone_msg1;KallitheaKlone_msg2;|;KallitheaKlone_LocalRevert;KallitheaKlone_Reclone;KallitheaKlone_Update;|;KallitheaKlone_Settings", RegistryValueKind.String);
                }
            }

            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"Directory\shell", true))
            {
                using (RegistryKey subKey = key.CreateSubKey("KallitheaKlone", RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    subKey.SetValue("MUIVerb", "Open Kallithea Klone here", RegistryValueKind.String);
                    subKey.SetValue("Icon", "\"" + location + "\"", RegistryValueKind.String);
                    using (RegistryKey commandKey = subKey.CreateSubKey("command", RegistryKeyPermissionCheck.ReadWriteSubTree))
                    {
                        commandKey.SetValue("", "\"" + location + "\" Clone \"%V\"");
                    }
                }

                using (RegistryKey subKey = key.CreateSubKey("KallitheaKlone_Other", RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    subKey.SetValue("MUIVerb", "Other Kallithea Klone Options", RegistryValueKind.String);
                    subKey.SetValue("SubCommands", "KallitheaKlone_msg1;KallitheaKlone_msg2;|;KallitheaKlone_LocalRevert;KallitheaKlone_Reclone;KallitheaKlone_Update;|;KallitheaKlone_Settings", RegistryValueKind.String);
                }
            }

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\Shell", true))
            {
                using (RegistryKey subKey = key.CreateSubKey("KallitheaKlone_msg1", RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    subKey.SetValue("MUIVerb", "The options below are all EXPERIMENAL!", RegistryValueKind.String);
                }

                using (RegistryKey subKey = key.CreateSubKey("KallitheaKlone_msg2", RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    subKey.SetValue("MUIVerb", "But please test them where it is safe :)", RegistryValueKind.String);
                }

                using (RegistryKey subKey = key.CreateSubKey("KallitheaKlone_LocalRevert", RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    subKey.SetValue("MUIVerb", "Revert all uncommited changes", RegistryValueKind.String);
                    subKey.SetValue("Icon", "\"" + location + "\"", RegistryValueKind.String);
                    using (RegistryKey commandKey = subKey.CreateSubKey("command", RegistryKeyPermissionCheck.ReadWriteSubTree))
                    {
                        commandKey.SetValue("", "\"" + location + "\" LocalRevert \"%V\"");
                    }
                }

                using (RegistryKey subKey = key.CreateSubKey("KallitheaKlone_Reclone", RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    subKey.SetValue("MUIVerb", "Delete and re-clone", RegistryValueKind.String);
                    subKey.SetValue("Icon", "\"" + location + "\"", RegistryValueKind.String);
                    using (RegistryKey commandKey = subKey.CreateSubKey("command", RegistryKeyPermissionCheck.ReadWriteSubTree))
                    {
                        commandKey.SetValue("", "\"" + location + "\" Reclone \"%V\"");
                    }
                }

                using (RegistryKey subKey = key.CreateSubKey("KallitheaKlone_Update", RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    subKey.SetValue("MUIVerb", "Update to latest commit", RegistryValueKind.String);
                    subKey.SetValue("Icon", "\"" + location + "\"", RegistryValueKind.String);
                    using (RegistryKey commandKey = subKey.CreateSubKey("command", RegistryKeyPermissionCheck.ReadWriteSubTree))
                    {
                        commandKey.SetValue("", "\"" + location + "\" Update \"%V\"");
                    }
                }

                using (RegistryKey subKey = key.CreateSubKey("KallitheaKlone_Settings", RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    subKey.SetValue("MUIVerb", "Settings", RegistryValueKind.String);
                    subKey.SetValue("Icon", "\"" + location + "\"", RegistryValueKind.String);
                    using (RegistryKey commandKey = subKey.CreateSubKey("command", RegistryKeyPermissionCheck.ReadWriteSubTree))
                    {
                        commandKey.SetValue("", "\"" + location + "\" Settings \"%V\"");
                    }
                }
            }
        }

        private void Uninstall(string attempt = "0")
        {
            try
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

                using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"Directory\Background\shell", true))
                {
                    key.DeleteSubKeyTree("KallitheaKlone", false);
                    key.DeleteSubKeyTree("KallitheaKlone_Other", false);
                }

                using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"Directory\shell", true))
                {
                    key.DeleteSubKeyTree("KallitheaKlone", false);
                    key.DeleteSubKeyTree("KallitheaKlone_Other", false);
                }

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\Shell", true))
                {
                    foreach (string subkey in key.GetSubKeyNames())
                    {
                        if (subkey.StartsWith("KallitheaKlone"))
                            key.DeleteSubKeyTree(subkey, false);
                    }
                }
            }
            catch
            {

            }

            try
            {
                Default.Reset();
                Default.Save();
                foreach (string file in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Kallithea Klone"))
                {
                    File.Delete(file);
                }
                Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Kallithea Klone");
            }
            catch
            {

            }
        }

        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }   

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