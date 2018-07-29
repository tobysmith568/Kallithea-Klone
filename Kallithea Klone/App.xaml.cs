using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Kallithea_Klone
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                switch (UppercaseFirst(e.Args[0]))
                {
                    case nameof(RunTypes.Settings):
                        Settings();
                        break;
                    case nameof(RunTypes.Clone):
                        if (e.Args.Length >= 2)
                            Clone(e.Args[1]);
                        else
                            goto default;
                        break;
                    default:
                        Environment.Exit(0);
                        break;
                }
            }
            else
                Environment.Exit(0);
        }

        private void Settings()
        {
            throw new NotImplementedException();
        }

        private void Clone(string ranFrom)
        {
            MainWindow window = new MainWindow(ranFrom);
            window.Left = Cursor.Position.X;
            window.Top = Cursor.Position.Y;

            int windowHeight = (int)window.Height;
            int windowWidth = (int)window.Width;

            int screenHeight = Screen.FromPoint(Cursor.Position).Bounds.Height;
            int screenWidth = Screen.FromPoint(Cursor.Position).Bounds.Width;

            while (window.Top + windowHeight + 5 > screenHeight)
                window.Top -= 1;

            while (window.Left + windowWidth + 5 > screenWidth)
                window.Left -= 1;

            window.Show();
        }

        string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            return char.ToUpper(s[0]) + s.Substring(1);
        }
    }
}
