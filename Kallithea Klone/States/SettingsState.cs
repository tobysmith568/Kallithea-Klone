using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Kallithea_Klone.Other_Classes;

namespace Kallithea_Klone.States
{
    public class SettingsState : LogicOnlyState
    {
        //  State Methods
        //  =============

        public override void InitialActions()
        {
            Settings window = new Settings();

            int windowHeight = (int)window.Height;
            int windowWidth = (int)window.Width;

            window.Left = System.Windows.Forms.Cursor.Position.X - (windowWidth / 2);
            window.Top = System.Windows.Forms.Cursor.Position.Y - (windowHeight / 2);

            int screenHeight = System.Windows.Forms.Screen.FromPoint(System.Windows.Forms.Cursor.Position).Bounds.Height;
            int screenWidth = System.Windows.Forms.Screen.FromPoint(System.Windows.Forms.Cursor.Position).Bounds.Width;

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
