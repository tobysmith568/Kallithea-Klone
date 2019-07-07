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

        public Command<IChangeSet> LoadSelectedChangeSet { get; }

        //  Constructors
        //  ============

        public MainWindowViewModel()
        {
            Repositories = new ObservableCollection<IRepository>();

            LoadSelectedChangeSet = new Command<IChangeSet>(DoLoadSelectedChangeSet);
        }

        //  Methods
        //  =======

        private async void DoLoadSelectedChangeSet(IChangeSet changeSet)
        {
            await changeSet.Load();

            foreach (IFile file in changeSet.Files)
            {
                await file.Load();
            }
        }
    }
}
