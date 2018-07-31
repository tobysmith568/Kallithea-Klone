using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Kallithea_Klone
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.APIKey = TbxAPIKey.Text;
            MainWindow.Host = TbxHost.Text;
            MainWindow.Email = TbxEmail.Text;
            MainWindow.Password = PbOne.Password;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TbxAPIKey.Text = MainWindow.APIKey;
            TbxHost.Text = MainWindow.Host;
            TbxEmail.Text = MainWindow.Email;
            PbOne.Password = MainWindow.Password;
            PbTwo.Password = MainWindow.Password;

            PbOne.PasswordChanged += PasswordChanged;
            PbTwo.PasswordChanged += PasswordChanged;
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

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            LblNotMatching.Visibility = PbOne.Password == PbTwo.Password ? Visibility.Hidden : Visibility.Visible;
            BtnSave.IsEnabled = GetSaveButtonEnabled();
        }

        private bool GetSaveButtonEnabled()
        {
            if (PbOne.Password != PbTwo.Password)
                return false;

            return true;
        }
    }
}
