using Microsoft.Win32;
namespace Kallithea_Klone.ContextMenu
{
    public class MenuParent : IMenuItem
    {
        //  Properties
        //  ==========

        public Child[] Children { get; set; } = new Child[0];
        public MenuLocation LocationType { get; set; } = MenuLocation.Both;

        public string Name { get; set; }
        public string Label { get; set; } = "";
        public string Icon { get; set; } = "";
        public string Command { get; set; } = "";

        //  Methods
        //  =======

        /// <exception cref="System.ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public string Create()
        {
            string subCommands = "";
            foreach (Child child in Children)
            {
                subCommands += child.Create() + ";";
            }

            if (subCommands.Length > 0)
            {
                subCommands.Substring(0, subCommands.Length - 1);
            }

            if (LocationType == MenuLocation.Backgrounds || LocationType == MenuLocation.Both)
            {
                Create(@"Directory\Background\shell", subCommands);
            }

            if (LocationType == MenuLocation.Directories || LocationType == MenuLocation.Both)
            {
                Create(@"Directory\shell", subCommands);
            }

            return Name;
        }

        /// <exception cref="System.ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        private void Create(string registryLocation, string subCommands)
        {
            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(registryLocation, true))
            {
                using (RegistryKey subKey = key.CreateSubKey(Name, RegistryKeyPermissionCheck.ReadWriteSubTree))
                {
                    if (Label.Length > 0)
                        subKey.SetValue("MUIVerb", Label, RegistryValueKind.String);

                    if (Icon.Length > 0)
                        subKey.SetValue("Icon", Icon, RegistryValueKind.String);

                    if (subCommands.Length > 0)
                        subKey.SetValue("SubCommands", subCommands, RegistryValueKind.String);

                    if (Command.Length > 0)
                    {
                        using (RegistryKey commandKey = subKey.CreateSubKey("command", RegistryKeyPermissionCheck.ReadWriteSubTree))
                        {
                            commandKey.SetValue("", Command);
                        }
                    }
                }
            }
        }
    }
}
