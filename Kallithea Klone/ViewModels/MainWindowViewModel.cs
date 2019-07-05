using KallitheaKlone.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        //  Variables
        //  =========

        private ObservableCollection<IRepository> repositories;

        //  Properties
        //  ==========

        public ObservableCollection<IRepository> Repositories
        {
            get => repositories;
            set => PropertyChanging(value, ref repositories, nameof(Repositories));
        }

        //  Constructors
        //  ============

        public MainWindowViewModel()
        {
            Repositories = new ObservableCollection<IRepository>();
        }

        //  Methods
        //  =======

        /*
         
        DoWhatever

         */
    }
}
