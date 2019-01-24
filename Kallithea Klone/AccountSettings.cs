using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kallithea_Klone
{
    public class AccountSettings : IAccountSettings
    {
        //  Variables
        //  =========

        private static readonly IAccountSettings singleton;

        //  Constructors
        //  ============

        private AccountSettings()
        {

        }

        static AccountSettings()
        {
            singleton = new AccountSettings();
        }

        //  Static Properties
        //  =================

        public static string APIKey
        {
            get => singleton._APIKey;
            set => singleton._APIKey = value;
        }

        public static string Username
        {
            get => singleton._Username;
            set => singleton._Username = value;
        }

        public static string Host
        {
            get => singleton._Host;
            set => singleton._Host = value;
        }

        public static string Password
        {
            get => singleton._Password;
            set => singleton._Password = value;
        }

        public static bool Updates
        {
            get => singleton._Updates;
            set => singleton._Updates = value;
        }

        public static bool JustInstalled
        {
            get => singleton._JustInstalled;
            set => singleton._JustInstalled = value;
        }

        public static bool AdvancedOptions
        {
            get => singleton._AdvancedOptions;
            set => singleton._AdvancedOptions = value;
        }

        //  Properties
        //  ==========

        public string _APIKey
        {
            get
            {
                return Properties.Settings.Default.APIKey;
            }
            set
            {
                Properties.Settings.Default.APIKey = value;
                Properties.Settings.Default.Save();
            }
        }

        public string _Username
        {
            get
            {
                return Properties.Settings.Default.Username;
            }
            set
            {
                Properties.Settings.Default.Username = value;
                Properties.Settings.Default.Save();
            }
        }

        public string _Host
        {
            get
            {
                return Properties.Settings.Default.Host;
            }
            set
            {
                Properties.Settings.Default.Host = value;
                Properties.Settings.Default.Save();
            }
        }

        public string _Password
        {
            get
            {
                if (Properties.Settings.Default.Password == "")
                    return Properties.Settings.Default.Password;

                return Encoding.Unicode.GetString(ProtectedData.Unprotect(
                    Convert.FromBase64String(Properties.Settings.Default.Password),
                    null,
                    DataProtectionScope.LocalMachine));
            }
            set
            {
                Properties.Settings.Default.Password = Convert.ToBase64String(ProtectedData.Protect(
                    Encoding.Unicode.GetBytes(value),
                    null,
                    DataProtectionScope.LocalMachine));
                Properties.Settings.Default.Save();
            }
        }

        public bool _Updates
        {
            get
            {
                return Properties.Settings.Default.CheckForUpdates;
            }
            set
            {
                Properties.Settings.Default.CheckForUpdates = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool _JustInstalled
        {
            get
            {
                return Properties.Settings.Default.JustInstalled;
            }
            set
            {
                Properties.Settings.Default.JustInstalled = value;
                Properties.Settings.Default.Save();
            }
        }

        public bool _AdvancedOptions
        {
            get
            {
                return Properties.Settings.Default.AdvancedOptions;
            }
            set
            {
                Properties.Settings.Default.AdvancedOptions = value;
                Properties.Settings.Default.Save();
            }
        }

        //  Static Methods
        //  ==============

        public static void Upgrade()
        {
            singleton._Upgrade();
        }

        public static void Reset()
        {
            singleton._Reset();
        }

        public static void Save()
        {
            singleton._Save();
        }

        public static bool VerifySettings()
        {
            return singleton._VerifySettings();
        }

        //  Methods
        //  =======

        public void _Upgrade()
        {
            Properties.Settings.Default.Upgrade();
        }

        public void _Reset()
        {
            Properties.Settings.Default.Reset();
        }

        public void _Save()
        {
            Properties.Settings.Default.Save();
        }

        public bool _VerifySettings()
        {
            if (SettingsNotConfigured())
            {
                MessageBoxResult result = MessageBox.Show("It looks like you have not properly set up your settings.\n" +
                     "Would you like to open them now?", "Empty settings!", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);

                switch (result)
                {
                    case MessageBoxResult.OK:
                        return MainWindow.OpenSettings();
                    default:
                        return false;
                }
            }
            return true;
        }

        private bool SettingsNotConfigured()
        {
            return AccountSettings.Host == ""
                || AccountSettings.APIKey == ""
                || AccountSettings.Username == ""
                || AccountSettings.Password == "";
        }
    }
}