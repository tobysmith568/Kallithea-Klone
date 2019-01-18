using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Kallithea_Klone
{
    /// <summary>
    /// Interaction logic for UpdateFound.xaml
    /// </summary>
    public partial class UpdateFound : UserControl
    {
        //  Variables
        //  =========

        private UpdateWindow parent;

        //  Constructors
        //  ============

        public UpdateFound(UpdateWindow parent)
        {
            this.parent = parent;

            InitializeComponent();
        }

        //  Events
        //  ======

        private void BtnGithub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(parent.GithubURL));
            Environment.Exit(0);
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            parent.Download();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            parent.Close();
        }
    }
}
