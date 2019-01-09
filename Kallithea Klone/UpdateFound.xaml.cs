using System;
using System.Collections.Generic;
using System.Diagnostics;
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
