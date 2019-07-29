using KallitheaKlone.Models.Repositories;
using KallitheaKlone.ViewModels;
using System;
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

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(e.Source is TabControl))
            {
                return;
            }

            TabControl tabControl = e.Source as TabControl;

            ((TabViewModel)tabControl.SelectedItem).OnFocus.DoExecute(e);
        }


        //  Methods
        //  =======

        private void CloseTab_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button))
            {
                throw new ArgumentException("Argument needs to be of type [Button]", nameof(sender));
            }

            Button button = (Button)sender;

            if (button.DataContext == null || !(button.DataContext is RepositoryViewModel))
            {
                throw new ArgumentException("Button needs to have an dataContext of type [RepositoryViewModel]", nameof(sender));
            }

            RepositoryViewModel viewModel = (RepositoryViewModel)button.DataContext;

            VMCloseTab.Command.Execute(viewModel.URI);
        }
    }
}