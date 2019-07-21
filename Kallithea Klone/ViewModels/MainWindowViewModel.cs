using KallitheaKlone.Models.Dialogs.FolderPicker;
using KallitheaKlone.Models.Dialogs.RepositoryPicker;
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

        private readonly IRepositoryPicker repositoryPicker;

        private ObservableCollection<IRepository> repositories;
        private bool openDialogVisibility;

        //  Properties
        //  ==========

        public ObservableCollection<IRepository> Repositories
        {
            get => repositories;
            set => PropertyChanging(value, ref repositories, nameof(Repositories));
        }

        public bool OpenDialogVisibility
        {
            get => openDialogVisibility;
            set => PropertyChanging(value, ref openDialogVisibility, nameof(OpenDialogVisibility));
        }



        public Command<IChangeSet> LoadSelectedChangeSet { get; }
        public Command ShowOpenDialogVisibility { get; }
        public Command HideOpenDialogVisibility { get; }
        public Command OpenNewRepository { get; }

        //  Constructors
        //  ============

        /// <exception cref="VersionControlException"></exception>
        public MainWindowViewModel()
        {
            Repositories = new ObservableCollection<IRepository>();

            LoadSelectedChangeSet = new Command<IChangeSet>(DoLoadSelectedChangeSet);
            ShowOpenDialogVisibility = new Command(DoShowOpenDialogVisibility);
            HideOpenDialogVisibility = new Command(DoHideOpenDialogVisibility);
            OpenNewRepository = new Command(DoOpenNewRepository);
        }

        public MainWindowViewModel(IRepositoryPicker repositoryPicker) : this()
        {
            this.repositoryPicker = repositoryPicker ?? throw new ArgumentNullException(nameof(repositoryPicker));
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

        private void DoShowOpenDialogVisibility() => OpenDialogVisibility = true;

        private void DoHideOpenDialogVisibility() => OpenDialogVisibility = false;

        /// <exception cref="VersionControlException"></exception>
        private async void DoOpenNewRepository()
        {
            IRepository newRepository = repositoryPicker.Select();

            if (newRepository != null)
            {
                OpenDialogVisibility = false;
                Repositories.Add(newRepository);
                await newRepository.Load();
            }
        }
    }
}