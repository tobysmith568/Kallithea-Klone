using KallitheaKlone.Models.Repositories;
using KallitheaKlone.ViewModels;
using KallitheaKlone.WPF.Models.Repositoties.Mercurial;
using System.Collections.ObjectModel;

namespace KallitheaKlone.WPF.ViewModels
{
    public class MainWindowMockViewModel : MainWindowViewModel
    {
        //  Constructors
        //  ============

        public MainWindowMockViewModel()
        {
            Repositories = new ObservableCollection<IRepository>()
            {
                new Repository(@"D:\Users\Toby\Downloads\V21product", "V22 Product")
                {
                }
            };
        }
    }
}
