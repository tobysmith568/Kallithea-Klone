using KallitheaKlone.Models.Repositories;
using KallitheaKlone.Models.Runner;
using KallitheaKlone.ViewModels;
using KallitheaKlone.WPF.Models.Repositories.Mercurial;
using KallitheaKlone.WPF.Models.Runner;
using System.Collections.ObjectModel;

namespace KallitheaKlone.WPF.ViewModels
{
    public class MainWindowMockViewModel : MainWindowViewModel
    {
        //  Constructors
        //  ============

        public MainWindowMockViewModel()
        {
            RepositoryViewModel repositoryViewModel = new RepositoryMockViewModel();

            Tabs = new ObservableCollection<TabViewModel>()
            {
                repositoryViewModel
            };
        }
    }
}
