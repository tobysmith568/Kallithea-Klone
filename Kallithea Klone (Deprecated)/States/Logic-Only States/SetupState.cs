using Kallithea_Klone.Other_Classes;
using System.Windows;

namespace Kallithea_Klone.States
{
    public class SetupState : LogicOnlyState
    {
        public override void InitialActions(string[] args)
        {
            if (!IsAdministrator())
            {
                if (args.Length < 1 || args[0] != "restarted")
                    RestartAsAdmin("Setup restarted");
                else
                {
                    MessageBoxResult result = MessageBox.Show("This installer needs administrator permissions in order to edit your Windows Explorer Menus.\n" +
                        "This is essential for the primary functionality. Press OK to try again, or Cancel to install without this essential functionality",
                        "Kallithea Klone needs administrator permissions!", MessageBoxButton.OKCancel, MessageBoxImage.Error);

                    switch (result)
                    {
                        case MessageBoxResult.OK:
                            RestartAsAdmin("Setup restarted");
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
                MessageBox.Show("Kallithea Klone was unable to add all of the Windows Explorer context menu items it uses.\n" +
                    "Please re-run the installer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            App.Current.Shutdown();
        }
    }
}
