using Kallithea_Klone.ContextMenu;
using Microsoft.Win32;
using System.Linq;
using System.Windows;

namespace Kallithea_Klone.Other_Classes
{
    public static class ContextMenuImplementations
    {
        //  Variables
        //  =========

        private static readonly string programLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;

        private static readonly MenuParent standard = new MenuParent()
        {
            Name = "KallitheaKlone",
            Label = "Open Kallithea Klone here",
            Icon = "\"" + programLocation + "\"",
            Command = "\"" + programLocation + "\" Clone \"%V\"",
            LocationType = MenuLocation.Both
        };

        private static readonly MenuParent advancedOptions = new MenuParent()
        {
            Name = "KallitheaKlone_Other",
            Label = "Other Kallithea Klone Options",
            LocationType = MenuLocation.Both,
            Children = new Child[]
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
                new BreakChild(),
                new MenuChild()
                {
                    Name = "KallitheaKlone_LocalRevert",
                    Label = "Revert all uncommited changes",
                    Icon = "\"" + programLocation + "\"",
                    Command = "\"" + programLocation + "\" LocalRevert \"%V\""
                },
                new MenuChild()
                {
                    Name = "KallitheaKlone_Reclone",
                    Label = "Delete and re-clone",
                    Icon = "\"" + programLocation + "\"",
                    Command = "\"" + programLocation + "\" Reclone \"%V\""
                },
                new MenuChild()
                {
                    Name = "KallitheaKlone_Update",
                    Label = "Update to latest commit",
                    Icon = "\"" + programLocation + "\"",
                    Command = "\"" + programLocation + "\" Update \"%V\""
                },
                new BreakChild(),
                new MenuChild()
                {
                    Name = "KallitheaKlone_Settings",
                    Label = "Settings",
                    Icon = "\"" + programLocation + "\"",
                    Command = "\"" + programLocation + "\" Settings \"%V\""
                }
            }
        };

        //  Methods
        //  =======

        /// <exception cref="System.ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public static void CreateStandard()
        {
            standard.Create();
        }

        /// <exception cref="System.ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public static void CreateAdvanced()
        {
            advancedOptions.Create();
        }

        /// <exception cref="System.ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public static void RemoveAll()
        {
            string[] locations = new string[]
            {
                @"Directory\Background\shell",
                @"Directory\shell",
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\Shell"
            };

            foreach (string location in locations)
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(location, true))
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
