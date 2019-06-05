using Kallithea_Klone.Account_Settings;
using Kallithea_Klone.ContextMenu;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Kallithea_Klone.Other_Classes
{
    public static class ContextMenuImplementations
    {
        //  Variables
        //  =========

        private static readonly string programLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;

        private static readonly MenuParent standardMenuItem = new MenuParent()
        {
            Name = "KallitheaKlone",
            Label = "Open Kallithea Klone here",
            Icon = "\"" + programLocation + "\"",
            Command = "\"" + programLocation + "\" Clone \"%V\"",
            LocationType = MenuLocation.Both
        };

        //  Methods
        //  =======

        /// <exception cref="System.ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public static void CreateStandard()
        {
            standardMenuItem.Create();
        }

        /// <exception cref="System.ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public static void CreateAdvanced()
        {
            AdvancedOptions advancedOptions = AccountSettings.AdvancedOptions;

            if (!advancedOptions.Enabled)
                return;

            List<Child> enabledChildren = new List<Child>
            {
                new MenuChild()
                {
                    Name = "KallitheaKlone_msg1",
                    Label = "The options below are all EXPERIMENAL!"
                },
                new MenuChild()
                {
                    Name = "KallitheaKlone_msg2",
                    Label = "But please test them where it is safe :)"
                },
                new BreakChild()
            };

            if (advancedOptions.Revert)
            {
                enabledChildren.Add(
                    new MenuChild()
                    {
                        Name = "KallitheaKlone_LocalRevert",
                        Label = "Revert all uncommited changes",
                        Icon = "\"" + programLocation + "\"",
                        Command = "\"" + programLocation + "\" LocalRevert \"%V\""
                    }
                );
            }
            
            if (advancedOptions.Reclone)
            {
                enabledChildren.Add(
                    new MenuChild()
                    {
                        Name = "KallitheaKlone_Reclone",
                        Label = "Delete and re-clone",
                        Icon = "\"" + programLocation + "\"",
                        Command = "\"" + programLocation + "\" Reclone \"%V\""
                    }
                );
            }
            
            if (advancedOptions.Update)
            {
                enabledChildren.Add(
                    new MenuChild()
                    {
                        Name = "KallitheaKlone_Update",
                        Label = "Update to latest commit",
                        Icon = "\"" + programLocation + "\"",
                        Command = "\"" + programLocation + "\" Update \"%V\""
                    }
                );
            }
            
            if (advancedOptions.Settings)
            {
                enabledChildren.Add(new BreakChild());
                enabledChildren.Add(
                    new MenuChild()
                    {
                        Name = "KallitheaKlone_Settings",
                        Label = "Settings",
                        Icon = "\"" + programLocation + "\"",
                        Command = "\"" + programLocation + "\" Settings \"%V\""
                    }
                );
            }

            MenuParent advancedMenuItems = new MenuParent()
            {
                Name = "KallitheaKlone_Other",
                Label = "Other Kallithea Klone Options",
                LocationType = MenuLocation.Both,
                Children = enabledChildren.ToArray()
            };

            advancedMenuItems.Create();
        }

        /// <exception cref="System.ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public static void RemoveAll()
        {
            string[] localMachineOptions = new string[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\Shell"
            };

            RemoveAllFromLocation(Registry.LocalMachine, localMachineOptions);

            string[] classesRootOptions = new string[]
            {
                @"Directory\Background\shell",
                @"Directory\shell"
            };

            RemoveAllFromLocation(Registry.ClassesRoot, classesRootOptions);
        }

        /// <exception cref="System.ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        private static void RemoveAllFromLocation(RegistryKey parentKey, string[] locations)
        {
            foreach (string location in locations)
            {
                using (RegistryKey key = parentKey.OpenSubKey(location, true))
                {
                    foreach (string subkey in key.GetSubKeyNames().Where(n => n.StartsWith("KallitheaKlone")))
                    {
                        key.DeleteSubKeyTree(subkey, false);
                    }
                }
            }
        }
    }
}
