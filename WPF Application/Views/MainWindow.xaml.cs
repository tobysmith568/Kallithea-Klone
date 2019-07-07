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
        //  Constructors
        //  ============

        public MainWindow()
        {
            InitializeComponent();
        }

        //  Events
        //  ======

        /// <exception cref="VersionControlException"></exception>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void LeftColumnSplitter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                ResetPartition();
            }
        }

        private void ResetPartition()
        {
            Properties.MainWindow.Default.LeftColumnWidth = 150;
            Properties.MainWindow.Default.Save();
        }

        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            ICollection<IChangeSet> processedChangeSets = new List<IChangeSet>();
            foreach (DataGridCellInfo cell in e.AddedCells)
            {
                IChangeSet changeSet = (IChangeSet)cell.Item;

                if (processedChangeSets.Contains(changeSet))
                {
                    continue;
                }

                VMLoadSelectedChangeSetEvent.Command.Execute(changeSet);

                processedChangeSets.Add(changeSet);
            }
        }
    }
}