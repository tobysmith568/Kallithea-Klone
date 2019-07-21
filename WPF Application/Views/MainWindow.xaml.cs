﻿using KallitheaKlone.Models.Repositories;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
                ResetColumnPartition();
            }
        }

        private void BottomRowSplitter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                ResetRowPartition();
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