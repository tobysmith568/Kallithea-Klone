using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.ViewModels.Tabs
{
    public class NewTabViewModel : TabViewModel
    {
        //  Properties
        //  ==========

        public override bool IsClosable => false;

        public override string URI => Models.URIs.URI.NewTab.Value;

        public override string Name => "+";

        public override Command OnFocus { get; }

        public MainWindowViewModel MainWindowViewModel { get; set; }

        //  Constructors
        //  ============

        /// <exception cref="InvalidOperationException"></exception>
        public NewTabViewModel()
        {
            OnFocus = new Command(DoOnFocus);
        }

        //  Methods
        //  =======

        /// <exception cref="InvalidOperationException"></exception>
        private void DoOnFocus()
        {
            if (MainWindowViewModel == null)
            {
                throw new InvalidOperationException($"The [{nameof(MainWindowViewModel)}] needs to not be null in order to open a new tab");
            }

            MainWindowViewModel.Tabs.Add(Models.URIs.URI.OpenRepository.ViewModel.Invoke());
            MainWindowViewModel.SelectedRepositoryIndex = MainWindowViewModel.Tabs.Count - 1;
        }
    }
}
