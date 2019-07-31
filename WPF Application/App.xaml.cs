using KallitheaKlone.Models.Dialogs.FolderPicker;
using KallitheaKlone.Models.Dialogs.MessagePrompts;
using KallitheaKlone.Models.Dialogs.RepositoryPicker;
using KallitheaKlone.Models.Repositories;
using KallitheaKlone.ViewModels;
using KallitheaKlone.ViewModels.Tabs;
using KallitheaKlone.WPF.Models.Dialogs.FolderPicker;
using KallitheaKlone.WPF.Models.Dialogs.MessagePrompts;
using KallitheaKlone.WPF.Models.Dialogs.RepositoryPicker;
using KallitheaKlone.WPF.Models.Repositories;
using KallitheaKlone.WPF.Views;
using System;
using System.Windows;
using Unity;

namespace WPF_Application
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //  Variables
        //  =========

        private readonly IUnityContainer container = new UnityContainer();

        //  Events
        //  ======

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SetUpImplementations();
            ShowMainWindow();
        }

        //  Methods
        //  =======

        private void SetUpImplementations()
        {
            container.RegisterType<IMessagePrompt, MessagePrompt>();
            container.RegisterType<IFolderPicker, FolderPicker>();
            container.RegisterType<IRepositoryPicker, RepositoryPicker>();
            container.RegisterType<IRepositoryFactory, RepositoryFactory>();
        }

        /// <exception cref="InvalidOperationException"></exception>
        private void ShowMainWindow()
        {
            MainWindowViewModel mainWindowViewModel = container.Resolve<MainWindowViewModel>();
            mainWindowViewModel.NewTab.Add(new NewTabViewModel()
            {
                MainWindowViewModel = mainWindowViewModel
            });
            mainWindowViewModel.Tabs.Add(new OpenRepositoryViewModel());

            var window = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
            window.Show();
        }
    }
}
