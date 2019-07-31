using KallitheaKlone.Models.Dialogs.RepositoryPicker;
using KallitheaKlone.Models.Repositories;
using KallitheaKlone.Models.URIs;
using KallitheaKlone.ViewModels.Tabs;
using System;
using System.Collections.ObjectModel;

namespace KallitheaKlone.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        //  Variables
        //  =========

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

        public Command ShowOpenDialogVisibility { get; }
        public Command HideOpenDialogVisibility { get; }
        public Command<string> OpenNewInternalTab { get; }
        public Command<string> CloseRepository { get; }

        //  Constructors
        //  ============

        /// <exception cref="VersionControlException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public MainWindowViewModel()
        {
            Tabs = new ObservableCollection<TabViewModel>();
            NewTab = new ObservableCollection<TabViewModel>();

            ShowOpenDialogVisibility = new Command(DoShowOpenDialogVisibility);
            HideOpenDialogVisibility = new Command(DoHideOpenDialogVisibility);
            OpenNewInternalTab = new Command<string>(DoOpenNewInternalTab);
            CloseRepository = new Command<string>(DoCloseRepository);
        }

        //  Methods
        //  =======

        private void DoShowOpenDialogVisibility() => OpenDialogVisibility = true;

        private void DoHideOpenDialogVisibility() => OpenDialogVisibility = false;

        /// <exception cref="VersionControlException"></exception>
        private void DoOpenNewInternalTab(string url)
        {
            Uri uri = new Uri(url ?? throw new ArgumentNullException(nameof(url)));

            if (uri.Scheme != URI.InternalScheme)
            {
#warning //TODO LOG
                return;
            }

            foreach (URI internalURI in URI.GetAll<URI>())
            {
                if (url == internalURI.Value)
                {
                    Tabs.Add(internalURI.ViewModel.Invoke());
                }
            }
        }

        /// <exception cref="InvalidOperationException"></exception>
        private void DoCloseRepository(string uri)
        {
            if (Tabs.Count == 1)
            {
                throw new InvalidOperationException("You cannot close all of the open tabs");
            }

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