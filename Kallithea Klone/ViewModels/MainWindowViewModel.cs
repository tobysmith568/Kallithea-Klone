using KallitheaKlone.Models.Dialogs.RepositoryPicker;
using KallitheaKlone.Models.Repositories;
using KallitheaKlone.Models.URIs;
using KallitheaKlone.ViewModels.Tabs;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace KallitheaKlone.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        //  Variables
        //  =========

        private readonly IRepositoryPicker repositoryPicker;

        private ObservableCollection<TabViewModel> tabs;
        private ObservableCollection<TabViewModel> newTab;
        private int selectedRepositoryIndex;
        private bool openDialogVisibility;

        //  Properties
        //  ==========

        public ObservableCollection<TabViewModel> Tabs
        {
            get => tabs;
            set => PropertyChanging(value, ref tabs, nameof(Tabs));
        }

        public ObservableCollection<TabViewModel> NewTab
        {
            get => newTab;
            set => PropertyChanging(value, ref newTab, nameof(NewTab));
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

        public Command<string> OpenNewInternalTab { get; }
        public Command OpenNewRepositoryTab { get; }
        public Command<string> CloseRepository { get; }

        //  Constructors
        //  ============

        /// <exception cref="VersionControlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public MainWindowViewModel(IRepositoryPicker repositoryPicker) : this()
        {
            this.repositoryPicker = repositoryPicker;
        }

        /// <exception cref="VersionControlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public MainWindowViewModel()
        {
            Tabs = new ObservableCollection<TabViewModel>();
            NewTab = new ObservableCollection<TabViewModel>();

            OpenNewInternalTab = new Command<string>(DoOpenNewInternalTab);
            OpenNewRepositoryTab = new Command(DoOpenNewRepositoryTab);
            CloseRepository = new Command<string>(DoCloseRepository);
        }

        //  Methods
        //  =======

        /// <exception cref="VersionControlException"></exception>
        private void DoOpenNewInternalTab(string url)
        {
            Uri uri = new Uri(url ?? throw new ArgumentNullException(nameof(url)));

            if (uri.Scheme != URI.InternalScheme)
            {
#warning //TODO LOG
                return;
            }

            if (SelectTab(url))
            {
                return;
            }

            foreach (URI internalURI in URI.GetAll<URI>())
            {
                if (url == internalURI.Value)
                {
                    Tabs.Add(internalURI.ViewModel.Invoke(this));
                    SelectedRepositoryIndex = Tabs.Count - 1;
                }
            }
        }

        /// <exception cref="VersionControlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        private async void DoOpenNewRepositoryTab()
        {
            IRepository newRepository = repositoryPicker.Select();

            if (newRepository == null || SelectTab(newRepository.URI))
            {
                return;
            }

            RepositoryViewModel newViewModel = new RepositoryViewModel
            {
                RepositorySource = newRepository
            };

            Tabs.Add(newViewModel);
            DoCloseRepository(URI.OpenRepository.Value);

            SelectedRepositoryIndex = Tabs.Count - 1;
            await newRepository.Load();

        }

        /// <exception cref="InvalidOperationException"></exception>
        private void DoCloseRepository(string uri)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                if (Tabs[i].URI == uri)
                {
                    int tabToSelect = i - 1;
                    if (tabToSelect < 0)
                    {
                        tabToSelect = 0;
                    }

                    SelectedRepositoryIndex = tabToSelect;
                    Tabs.RemoveAt(i);
                    return;
                }
            }
        }

        private bool SelectTab(string uri)
        {
            for (int i = 0; i < Tabs.Count; i++)
            {
                if (Tabs[i].URI == uri)
                {
                    SelectedRepositoryIndex = i;
                    return true;
                }
            }
            return false;
        }
    }
}