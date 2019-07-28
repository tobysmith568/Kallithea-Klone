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

        private ObservableCollection<TabViewModel> tabs;
        private int selectedRepositoryIndex;
        private bool openDialogVisibility;

        //  Properties
        //  ==========

        public ObservableCollection<TabViewModel> Tabs
        {
            get => tabs;
            set => PropertyChanging(value, ref tabs, nameof(Tabs));
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



        public Command ShowOpenDialogVisibility { get; }
        public Command HideOpenDialogVisibility { get; }
        public Command OpenNewRepository { get; }
        public Command<string> CloseRepository { get; }

        //  Constructors
        //  ============

        /// <exception cref="VersionControlException"></exception>
        public MainWindowViewModel()
        {
            Tabs = new ObservableCollection<TabViewModel>();

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

        private void DoShowOpenDialogVisibility() => OpenDialogVisibility = true;

        private void DoHideOpenDialogVisibility() => OpenDialogVisibility = false;

        /// <exception cref="VersionControlException"></exception>
        private async void DoOpenNewRepository()
        {
            IRepository newRepository = repositoryPicker.Select();

            if (newRepository != null)
            {
                for (int i = 0; i < Tabs.Count; i++)
                {
                    if (!(Tabs[i] is RepositoryViewModel))
                    {
                        continue;
                    }

                    if (((RepositoryViewModel)Tabs[i]).RepositorySource.URI == newRepository.URI)
                    {
                        OpenDialogVisibility = false;
                        SelectedRepositoryIndex = i;
                        return;
                    }
                }

                OpenDialogVisibility = false;

                RepositoryViewModel newViewModel = new RepositoryViewModel
                {
                    RepositorySource = newRepository
                };

                Tabs.Add(newViewModel);

                SelectedRepositoryIndex = Tabs.Count - 1;
                await newRepository.Load();
            }
        }

        private void DoCloseRepository(string uri)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                if (Tabs[i].URI == uri)
                {
                    Tabs.RemoveAt(i);
                    break;
                }
            }
        }
    }
}