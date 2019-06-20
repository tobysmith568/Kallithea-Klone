using Kallithea_Klone.Account_Settings;
using Kallithea_Klone.Other_Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kallithea_Klone.States
{
    public class UninstallState : LogicOnlyState
    {
        public override void InitialActions(string[] args)
        {
            if (!IsAdministrator())
            {
                if (args.Length < 1 || args[0] != "restarted")
                    RestartAsAdmin("Uninstall restarted");
                else
                {
                    MessageBoxResult result = MessageBox.Show("This uninstaller need administrator permissions in order to edit your Windows Explorer Menus.\n" +
                        "This is essential to remove unused menu items. Press OK to try again or Cancel to leave the items after the uninstall.", "Kallithea Klone needs administrator permissions!", MessageBoxButton.OKCancel, MessageBoxImage.Error);

                    switch (result)
                    {
                        case MessageBoxResult.OK:
                            RestartAsAdmin("Uninstall restarted");
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
                MessageBox.Show("Kallithea Klone was unable to remove all Windows Explorer context menu items.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            AccountSettings.Reset();
            AccountSettings.Save();

            try
            {
                string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Kallithea Klone");
                foreach (string file in Directory.GetFiles(appDataFolder))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        //If app data cannot be removed then it will have to stay
                        //This try/catch is included as well as the outer one to
                        //Allow for as many files to be removed as possible
                    }
                }
                Directory.Delete(appDataFolder);
            }
            catch
            {
                //If app data cannot be removed then it will have to stay
            }
            App.Current.Shutdown();
        }
    }
}
