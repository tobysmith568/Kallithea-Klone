using KallitheaKlone.Models.Dialogs.MessagePrompts;
using KallitheaKlone.Models.JSONConverter;
using KallitheaKlone.Models.Repositories;
using KallitheaKlone.WPF.Models.Dialogs.MessagePrompts;
using KallitheaKlone.WPF.Models.JSONConverter;
using KallitheaKlone.WPF.Models.Repositoties.Mercurial;
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

namespace KallitheaKlone.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        /// <exception cref="VersionControlException"></exception>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
