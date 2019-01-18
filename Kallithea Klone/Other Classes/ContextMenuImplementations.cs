using Kallithea_Klone.ContextMenu;

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

        public static void CreateStandard()
        {
            standard.Create();
        }

        public static void CreateAdvanced()
        {
            advancedOptions.Create();
        }
    }
}
