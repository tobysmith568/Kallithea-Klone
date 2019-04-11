using Microsoft.Win32;

namespace Kallithea_Klone.ContextMenu
{
    public class MenuChild : Child, IMenuItem
    {
        //  Properties
        //  ==========

        public string Name { get; set; }
        public string Label { get; set; } = "";
        public string Icon { get; set; } = "";
        public string Command { get; set; } = "";

        //  Overridden Methods
        //  ==================

        /// <exception cref="System.ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public override string Create()
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\Shell", true))
            {
                using (RegistryKey subKey = key.CreateSubKey(Name, RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    if (Label.Length > 0)
                        subKey.SetValue("MUIVerb", Label, RegistryValueKind.String);

                    if (Icon.Length > 0)
                        subKey.SetValue("Icon", Icon, RegistryValueKind.String);

                    if (Command.Length > 0)
                    {
                        using (RegistryKey commandKey = subKey.CreateSubKey("command", RegistryKeyPermissionCheck.ReadWriteSubTree))
                        {
                            commandKey.SetValue("", Command);
                        }
                    }
                }
            }

            return Name;
        }
    }
}