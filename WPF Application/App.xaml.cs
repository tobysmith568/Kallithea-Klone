using KallitheaKlone.Models.Dialogs.MessagePrompts;
using KallitheaKlone.Models.Repositories;
using KallitheaKlone.ViewModels;
using KallitheaKlone.WPF.Models.Dialogs.MessagePrompts;
using KallitheaKlone.WPF.Models.Repositoties.Mercurial;
using KallitheaKlone.WPF.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
        protected async override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SetUpImplementations();
            await ShowMainWindow();
        }

        //  Methods
        //  =======

        private void SetUpImplementations()
        {
            container.RegisterType<IMessagePrompt, MessagePrompt>();
        }

        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="VersionControlException">Ignore.</exception>
        private async Task ShowMainWindow()
        {
            var mainWindowViewModel = container.Resolve<MainWindowViewModel>();
            var window = new MainWindow { DataContext = mainWindowViewModel };
            window.Show();

            IRepository repo = new Repository(@"D:\Users\Toby\Downloads\V21product", "V21 Product");
            await repo.Load();

            mainWindowViewModel.Repositories = new System.Collections.ObjectModel.ObservableCollection<IRepository>()
            {
                repo
            };
        }
    }
}
