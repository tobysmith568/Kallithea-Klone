using KallitheaKlone.Enums;
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

        public static URI NewTab = new URI(1, $"{InternalScheme}{SchemeSeparator}NewTab", new Func<TabViewModel>(() => new NewTabViewModel()));
        public static URI OpenRepository = new URI(2, $"{InternalScheme}{SchemeSeparator}OpenRepository", new Func<TabViewModel>(() => new OpenRepositoryViewModel()));

        //  Properties
        //  ==========

        public Func<TabViewModel> ViewModel { get; private set; }

        //  Constructors
        //  ============

        public URI(int id, string url, Func<TabViewModel> viewModel) : base(id, url)
        {
            ViewModel = viewModel;
        }
    }
}
