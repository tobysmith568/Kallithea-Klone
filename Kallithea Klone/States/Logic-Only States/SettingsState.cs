using System;
using System.Windows;
using WinForms = System.Windows.Forms;

namespace Kallithea_Klone.States
{
    public class SettingsState : LogicOnlyState
    {
        //  State Methods
        //  =============

        public override void InitialActions(string[] args)
        {
            Settings window = new Settings();

            int windowHeight = (int)window.Height;
            int windowWidth = (int)window.Width;

            window.Left = WinForms.Cursor.Position.X - (windowWidth / 2);
            window.Top = WinForms.Cursor.Position.Y - (windowHeight / 2);

            int screenHeight = WinForms.Screen.FromPoint(WinForms.Cursor.Position).Bounds.Height;
            int screenWidth = WinForms.Screen.FromPoint(WinForms.Cursor.Position).Bounds.Width;

            while (window.Top + windowHeight + 5 > screenHeight)
                window.Top -= 1;

            while (window.Top - 5 < 0)
                window.Top += 1;

            while (window.Left + windowWidth + 5 > screenWidth)
                window.Left -= 1;

            while (window.Left - 5 < 0)
                window.Left += 1;

            try
            {
                window.Show();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Unable to open the settings window!", "Unknown error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
