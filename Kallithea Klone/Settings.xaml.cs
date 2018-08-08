﻿using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kallithea_Klone
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
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
            TbxAPIKey.Text = MainWindow.APIKey;
            TbxHost.Text = MainWindow.Host;
            PbOne.Password = MainWindow.Password;
            PbTwo.Password = MainWindow.Password;

            PbOne.PasswordChanged += PasswordChanged;
            PbTwo.PasswordChanged += PasswordChanged;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveAndClose();
        }

        private void Tbx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                SaveAndClose();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(TbxHost.Text + "/_admin/my_account/api_keys", UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (result)
                Process.Start(new ProcessStartInfo(uriResult.AbsoluteUri));
            else
                MessageBox.Show("In order to find your API key you must correctly enter your host domain above.", "Incorrect Host!", MessageBoxButton.OK, MessageBoxImage.Error);

            e.Handled = true;
        }

        private void TbAPIKey_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;

            textBlock.TextDecorations.Clear();
        }

        private void TbAPIKey_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;

            textBlock.TextDecorations.Add(new TextDecoration(TextDecorationLocation.Underline, null, 0, TextDecorationUnit.Pixel, TextDecorationUnit.Pixel));
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            LblNotMatching.Visibility = PbOne.Password == PbTwo.Password ? Visibility.Hidden : Visibility.Visible;
            BtnSave.IsEnabled = GetSaveButtonEnabled();
        }

        private void BdrHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        //  Methods
        //  =======

        private bool GetSaveButtonEnabled()
        {
            if (PbOne.Password != PbTwo.Password)
                return false;

            return true;
        }

        private async void SaveAndClose()
        {
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
                        Response<User> user = JsonConvert.DeserializeObject<Response<User>>(response.Content);
                        if (user.Result == null)
                        {
                            MessageBox.Show("Error: " + user.Error, "Error!\t\t\t\t", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
                            BtnSave.IsEnabled = true;
                            GridCoverAll.Visibility = Visibility.Hidden;
                            break;
                        }
                        MainWindow.APIKey = TbxAPIKey.Text;
                        MainWindow.Host = TbxHost.Text;
                        MainWindow.Username = user.Result.Username;
                        MainWindow.Password = PbOne.Password;
                        Close();
                        break;
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
        }
    }
}
