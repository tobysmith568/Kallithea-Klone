using Kallithea_Klone.Account_Settings;
using Kallithea_Klone.States;
using System.Windows;
using System.Windows.Threading;
using System.Linq;
using System.IO;
using System;
using log4net.Config;
using log4net.Appender;
using log4net.Layout;
using log4net;

namespace Kallithea_Klone
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));
        //  Events
        //  ======

        /// <exception cref="System.Security.SecurityException">Ignore.</exception>
        void App_Startup(object sender, StartupEventArgs e)
        {
            log4net.Util.LogLog.InternalDebugging = true;
            PatternLayout  p = new PatternLayout()
            {
                ConversionPattern = "[%date{dd/MM/yyyy HH:mm:ss}] [%level] %message%newline"
            };
            p.ActivateOptions();
            RollingFileAppender a = new RollingFileAppender()
            {
                Layout = p,
                File = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Kallithea Klone", "Output.log"),
                RollingStyle = RollingFileAppender.RollingMode.Date,
                MaxSizeRollBackups = 5,
                ImmediateFlush = true,
                PreserveLogFileNameExtension = true,
                StaticLogFileName = true,
            };
            a.ActivateOptions();
            BasicConfigurator.Configure(a);

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