using KallitheaKlone.Models.Dialogs.RepositoryPicker;
using KallitheaKlone.Models.Repositories;
using KallitheaKlone.Models.Runner;
using KallitheaKlone.ViewModels;
using KallitheaKlone.ViewModels.Tabs;
using KallitheaKlone.WPF.Models.Repositories.Mercurial;
using KallitheaKlone.WPF.Models.Runner;
using KallitheaKlone.WPF.ViewModels.Tabs;
using System.Collections.ObjectModel;

namespace KallitheaKlone.WPF.ViewModels
{
    public class MainWindowMockViewModel : MainWindowViewModel
    {
        //  Constructors
        //  ============

        /// <exception cref="System.InvalidOperationException"></exception>
        public MainWindowMockViewModel(IRepositoryPicker repositoryPicker) : base(repositoryPicker)
        {
            RepositoryViewModel repositoryViewModel = new RepositoryMockViewModel();

            Tabs = new ObservableCollection<TabViewModel>()
            {
                repositoryViewModel
            };

            NewTab = new ObservableCollection<TabViewModel>()
            {
                new NewTabViewModel(this)
            };
        }

        public MainWindowMockViewModel() : this(null)
        {

        }
    }
}
