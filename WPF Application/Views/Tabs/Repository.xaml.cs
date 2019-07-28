using KallitheaKlone.Models.Repositories;
using KallitheaKlone.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KallitheaKlone.WPF.Views.Tabs
{
    /// <summary>
    /// Interaction logic for Repository.xaml
    /// </summary>
    public partial class Repository : UserControl
    {
        //  Constructors
        //  ============

        public Repository()
        {
            InitializeComponent();
        }

        //  Events
        //  ======

        private void LeftColumnSplitter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                ResetColumnPartition();
            }
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

        private void BottomRowSplitter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                ResetRowPartition();
            }
        }

        //  Methods
        //  =======

        private void ResetColumnPartition()
        {
            Properties.MainWindow.Default.LeftColumnWidth = 150;
            Properties.MainWindow.Default.Save();
        }

        private void ResetRowPartition()
        {
#warning //TODO
        }
    }
}
