using KallitheaKlone.Models.Dialogs.RepositoryPicker;
using KallitheaKlone.Models.Repositories;
using System;
using System.Collections.ObjectModel;

namespace KallitheaKlone.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        //  Variables
        //  =========

        private readonly IRepositoryPicker repositoryPicker;

        private ObservableCollection<IRepository> repositories;
        private int selectedRepositoryIndex;
        private bool openDialogVisibility;

        //  Properties
        //  ==========

        public ObservableCollection<IRepository> Repositories
        {
            get => repositories;
            set => PropertyChanging(value, ref repositories, nameof(Repositories));
        }

        public int SelectedRepositoryIndex
        {
            get => selectedRepositoryIndex;
            set => PropertyChanging(value, ref selectedRepositoryIndex, nameof(SelectedRepositoryIndex));
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
        public Command<string> CloseRepository { get; }

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
            CloseRepository = new Command<string>(DoCloseRepository);
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
                for (int i = 0; i < Repositories.Count; i++)
                {
                    if (Repositories[i].URI == newRepository.URI)
                    {
                        OpenDialogVisibility = false;
                        SelectedRepositoryIndex = i;
                        return;
                    }
                }

                OpenDialogVisibility = false;
                Repositories.Add(newRepository);
                SelectedRepositoryIndex = Repositories.Count - 1;
                await newRepository.Load();
            }
        }

        private void DoCloseRepository(string uri)
        {
            for (int i = 0; i < Repositories.Count; i++)
            {
                if (Repositories[i].URI == uri)
                {
                    Repositories.RemoveAt(i);
                    break;
                }
            }
        }
    }
}