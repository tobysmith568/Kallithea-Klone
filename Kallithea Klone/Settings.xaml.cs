using Kallithea_Klone.Account_Settings;
using Kallithea_Klone.Kallithea;
using Kallithea_Klone.Other_Classes;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Kallithea_Klone
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        //  Variables
        //  =========

        private int originalAdvancedSettingValue;
        string username = "";
        bool isFullySetUp = false;

        //  Constructors
        //  ============

        public Settings()
        {
            InitializeComponent();
        }

        //  Events
        //  ======

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GdAdminWarning.Visibility = Visibility.Hidden;

            TbxAPIKey.Text = AccountSettings.APIKey;
            TbxHost.Text = AccountSettings.Host;
            PbOne.Password = AccountSettings.Password;
            PbTwo.Password = AccountSettings.Password;

            PbOne.PasswordChanged += PasswordChanged;
            PbTwo.PasswordChanged += PasswordChanged;

            AdvancedOptions advancedOptions = AccountSettings.AdvancedOptions;
            originalAdvancedSettingValue = advancedOptions.PackedValue;
            CbAdvancedOptions.IsChecked = advancedOptions.Enabled;
            CbRevert.IsChecked = advancedOptions.Revert;
            CbReclone.IsChecked = advancedOptions.Reclone;
            CbUpdate.IsChecked = advancedOptions.Update;
            CbSettings.IsChecked = advancedOptions.Settings;

            CbCheckForUpdates.IsChecked = AccountSettings.Updates;

            isFullySetUp = true;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (await IsValidAccountInformation())
                SaveAndClose();
        }

        private async void Tbx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && await IsValidAccountInformation())
                SaveAndClose();
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        /// <exception cref="ObjectDisposedException">Ignore.</exception>
        /// <exception cref="System.IO.FileNotFoundException">Ignore.</exception>
        /// <exception cref="System.ComponentModel.Win32Exception">Ignore.</exception>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (ValidHost())
                Process.Start(new ProcessStartInfo(TbxHost.Text + "/_admin/my_account/api_keys"));
            else
                MessageBox.Show("In order to find your API key you must correctly enter your host domain above.", "Invalid Host!", MessageBoxButton.OK, MessageBoxImage.Error);

            e.Handled = true;
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!isFullySetUp)
                return;

            LblNotMatching.Visibility = PbOne.Password == PbTwo.Password ? Visibility.Hidden : Visibility.Visible;
            BtnSave.IsEnabled = GetSaveButtonEnabled();
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        private void BdrHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void Tbx_TextChanged(object sender, TextChangedEventArgs e)
        {
            BtnSave.IsEnabled = GetSaveButtonEnabled();
        }

        private void AdvancedOptionChanged(object sender, RoutedEventArgs e)
        {
            if (!isFullySetUp)
                return;

            GdAdminWarning.Visibility = AdvancedOptionsHaveChanged() ? Visibility.Visible : Visibility.Hidden;
        }

        //  Methods
        //  =======

        private bool GetSaveButtonEnabled()
        {
            if (TbxAPIKey.Text == "")
                return false;

            if (TbxHost.Text == "")
                return false;

            if (PbOne.Password == "")
                return false;

            if (PbTwo.Password == "")
                return false;

            if (PbOne.Password != PbTwo.Password)
                return false;

            return true;
        }

        private async Task<bool> IsValidAccountInformation()
        {
            if (!ValidHost())
            {
                MessageBox.Show("Your Host doesn't appear to be a fully formed URL.", "Incorrect Host!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            BtnSave.IsEnabled = false;
            GridCoverAll.Visibility = Visibility.Visible;

            RestClient client = new RestClient($"{TbxHost.Text}/_admin/api");
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\"id\":\"1\",\"api_key\":\"" + TbxAPIKey.Text + "\",\"method\":\"get_user\",\"args\":{}}", ParameterType.RequestBody);

            try
            {
                //Get the data async
                IRestResponse response = await Task.Run(() =>
                {
                    return client.Execute(request);
                });

                switch (response.ResponseStatus)
                {
                    case ResponseStatus.Completed:
                        KallitheaResponse<User> user = JsonConvert.DeserializeObject<KallitheaResponse<User>>(response.Content);
                        if (user.Result == null)
                        {
                            MessageBox.Show("Error: " + user.Error, "Error!\t\t\t\t", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                            BtnSave.IsEnabled = true;
                            GridCoverAll.Visibility = Visibility.Hidden;
                            return false;
                        }
                        username = user.Result.Username;
                        return true;
                    case ResponseStatus.TimedOut:
                        MessageBox.Show($"Webrequest to {response.ResponseUri} timed out", "Error!\t\t\t\t", MessageBoxButton.OK, MessageBoxImage.Error);
                        BtnSave.IsEnabled = true;
                        GridCoverAll.Visibility = Visibility.Hidden;
                        break;
                    case ResponseStatus.Error:
                    case ResponseStatus.Aborted:
                    default:
                        MessageBox.Show("Error: " + response.ErrorMessage, "Uncaught Error!\t\t\t\t", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                        BtnSave.IsEnabled = true;
                        GridCoverAll.Visibility = Visibility.Hidden;
                        break;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("Error: " + ee.Message, "Uncaught Error!", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                BtnSave.IsEnabled = true;
                GridCoverAll.Visibility = Visibility.Hidden;
            }

            return false;
        }

        private void SaveAndClose()
        {
            AccountSettings.APIKey = TbxAPIKey.Text;
            AccountSettings.Host = TbxHost.Text;
            AccountSettings.Username = username;
            AccountSettings.Password = PbOne.Password;
            AccountSettings.Updates = CbCheckForUpdates.IsChecked == true;
            AccountSettings.AdvancedOptions = new AdvancedOptions(
                enabled: CbAdvancedOptions.IsChecked == true,
                revert: CbRevert.IsChecked == true,
                reclone: CbReclone.IsChecked == true,
                update: CbUpdate.IsChecked == true,
                settings: CbSettings.IsChecked == true);

            if (AdvancedOptionsHaveChanged())
            {
                Process process;
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo(Process.GetCurrentProcess().MainModule.FileName)
                    {
                        Verb = "runas",
                        Arguments = "Setup"
                    };
                    process = Process.Start(startInfo);
                    process.WaitForExit();

                    if (process.ExitCode != 0)
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    MessageBox.Show("Unable to edit the Windows Explorer context menu options.\n" +
                        "If this continues please uninstall and re-install", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            
            if (System.Windows.Interop.ComponentDispatcher.IsThreadModal)
                DialogResult = true;

            Close();
        }

        private bool ValidHost()
        {
            return Uri.TryCreate(TbxHost.Text, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private bool AdvancedOptionsHaveChanged()
        {
            AdvancedOptions advancedOptions = new AdvancedOptions(
                enabled: CbAdvancedOptions.IsChecked == true,
                revert: CbRevert.IsChecked == true,
                reclone: CbReclone.IsChecked == true,
                update: CbUpdate.IsChecked == true,
                settings: CbSettings.IsChecked == true);

            return advancedOptions.PackedValue != originalAdvancedSettingValue;
        }
    }
}
