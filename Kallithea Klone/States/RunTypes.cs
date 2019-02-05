using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Kallithea_Klone.States
{
    public enum RunTypes
    {
        Clone,
        LocalRevert,
        Reclone,
        Update,

        Settings,
        
        Setup,
        Uninstall,
    }

    public static class RunTypeMethods
    {
        //  Extension Methods
        //  =================

        public static void Open(this RunTypes type, string[] args)
        {
            switch (type)
            {
                case RunTypes.Clone:
                case RunTypes.LocalRevert:
                case RunTypes.Reclone:
                case RunTypes.Update:
                    string runLocation;

                    if (args.Length > 0)
                    {
                        runLocation = args[0];
                    }
                    else if (CommonFileDialog.IsPlatformSupported)
                    {
                        runLocation = SelectWinVistaFolder();
                    }
                    else
                    {
                        runLocation = SelectWinXPFolder();
                    }

                    if (!Directory.Exists(runLocation))
                    {
                        System.Windows.MessageBox.Show("Given location is not a valid directory!",
                            "Command Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        return;
                    }

                    IState state = type.GetState(runLocation);
                    state.InitialActions();

                    OpenMainWindow(state);
                    return;

                case RunTypes.Settings:
                case RunTypes.Setup:
                case RunTypes.Uninstall:
                    type.GetState("").InitialActions();
                    return;
                default:
                    throw new NotImplementedException($"The runType {type.ToString()} has not been implemented.");
            }
        }

        private static IState GetState(this RunTypes type, string runLocation)
        {
            switch (type)
            {
                case RunTypes.Clone:
                    return new CloneState(runLocation);
                case RunTypes.LocalRevert:
                    return new LocalRevertState(runLocation);
                case RunTypes.Reclone:
                    return new ReCloneState(runLocation);
                case RunTypes.Update:
                    return new UpdateState(runLocation);
                case RunTypes.Settings:
                    return new SettingsState();
                case RunTypes.Setup:
                case RunTypes.Uninstall:
                default:
                    throw new NotImplementedException($"The runType {type.ToString()} has not been implemented.");
            }
        }

        //  Other Methods
        //  =============

        /// <exception cref="System.Security.SecurityException">Ignore.</exception>
        private static string SelectWinXPFolder()
        {
            using (FolderBrowserDialog   folderBrowserDialog = new FolderBrowserDialog
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
        private static string SelectWinVistaFolder()
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

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        /// <exception cref="Exception">Ignore.</exception>
        private static void OpenMainWindow(IState state)
        {
            MainWindow window = new MainWindow(state);

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
    }
}
