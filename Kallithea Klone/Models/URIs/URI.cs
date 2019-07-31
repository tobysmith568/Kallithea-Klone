using KallitheaKlone.Enums;
using KallitheaKlone.Models.Dialogs.RepositoryPicker;
using KallitheaKlone.ViewModels;
using KallitheaKlone.ViewModels.Tabs;
using System;

namespace KallitheaKlone.Models.URIs
{
    public class URI : Enumeration
    {
        //  Constants
        //  =========

        public const string InternalScheme = "internal";
        private const string SchemeSeparator = "://";

        //  Variables
        //  =========

        public static URI NewTab = new URI(1, $"{InternalScheme}{SchemeSeparator}NewTab",
            new Func<MainWindowViewModel, TabViewModel>((mainWindowViewModel) => new NewTabViewModel(mainWindowViewModel)));

        public static URI OpenRepository = new URI(2, $"{InternalScheme}{SchemeSeparator}OpenRepository",
            new Func<MainWindowViewModel, TabViewModel>((mainWindowViewModel) => new OpenRepositoryViewModel(mainWindowViewModel)));

        //  Properties
        //  ==========

        public Func<MainWindowViewModel, TabViewModel> ViewModel { get; private set; }

        //  Constructors
        //  ============

        public URI(int id, string url, Func<MainWindowViewModel, TabViewModel> viewModel) : base(id, url)
        {
            ViewModel = viewModel;
        }
    }
}
