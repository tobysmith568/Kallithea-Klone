using KallitheaKlone.Models.Dialogs.MessagePrompts;
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
        }

        /// <exception cref="InvalidOperationException"></exception>
        private void ShowMainWindow()
        {
            var mainWindowViewModel = container.Resolve<MainWindowViewModel>();
            var window = new MainWindow { DataContext = mainWindowViewModel };
            window.Show();

            mainWindowViewModel.Repositories = new System.Collections.ObjectModel.ObservableCollection<KallitheaKlone.Models.Repositories.IRepository>()
            {
                new Repository(@"D:\Users\Toby\Downloads\V21product", "V21 Product")
            };
        }
    }
}
