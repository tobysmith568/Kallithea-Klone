using Kallithea_Klone.Account_Settings;
using Kallithea_Klone.States;
using System.Windows;
using System.Windows.Threading;
using System.Linq;
using System.IO;
using System;
using Kallithea_Klone.Other_Classes;

namespace Kallithea_Klone
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Logger log = new Logger(typeof(App));
        //  Events
        //  ======

        /// <exception cref="System.Security.SecurityException">Ignore.</exception>
        void App_Startup(object sender, StartupEventArgs e)
        {
            if (AccountSettings.JustInstalled)
            {
                AccountSettings.Upgrade();
                AccountSettings.JustInstalled = false;
            }

            if (e.Args.Length == 0)
            {
                RunTypes.Clone.Open(new string[0]);
                return;
            }

            if (Enum.TryParse(UppercaseFirst(e.Args[0]), out RunTypes runType))
            {
                runType.Open(e.Args.Skip(1).ToArray());
                return;
            }

            MessageBox.Show($"Invalid initial argument [{e.Args[0]}] given!",
                "Command Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <exception cref="System.Security.SecurityException"></exception>
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            MessageBox.Show("An unexpected exception has occurred. Shutting down the application:\n" + args.Exception);

            // Prevent default unhandled exception processing
            args.Handled = true;

            Environment.Exit(0);
        }

        //  Methods
        //  =======

        private string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}