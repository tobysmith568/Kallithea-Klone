using KallitheaKlone.Models.Dialogs.FolderPicker;
using Microsoft.WindowsAPICodePack.Dialogs;
using WinForms = System.Windows.Forms;

namespace KallitheaKlone.WPF.Models.Dialogs.FolderPicker
{
    public class FolderPicker : IFolderPicker
    {
        //  Constants
        //  =========

        private const string SelectATargetFolder = "Select a target folder";
        private const string CDrive = @"C:\";

        //  Methods
        //  =======

        public string Show()
        {
            if (CommonFileDialog.IsPlatformSupported)
            {
                return SelectWinVistaOrLaterFolder();
            }
            else
            {
                return SelectWinXPFolder();
            }
        }

        private string SelectWinXPFolder()
        {
            using (WinForms.FolderBrowserDialog folderBrowserDialog = new WinForms.FolderBrowserDialog
            {
                Description = SelectATargetFolder
            })
            {
                WinForms.DialogResult result = folderBrowserDialog.ShowDialog();

                switch (result)
                {
                    case WinForms.DialogResult.OK:
                    case WinForms.DialogResult.Yes:
                        return folderBrowserDialog.SelectedPath;

                    default:
                        return null;
                }

            }
        }

        private string SelectWinVistaOrLaterFolder()
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                Title = SelectATargetFolder,
                IsFolderPicker = true,
                DefaultDirectory = CDrive,
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
                        return dialog.FileName;

                    default:
                        return null;
                }
            }
        }
    }
}
