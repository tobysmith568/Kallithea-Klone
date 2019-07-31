using KallitheaKlone.Models.Dialogs.RepositoryPicker;
using KallitheaKlone.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.ViewModels.Tabs
{
    public class OpenRepositoryViewModel : TabViewModel
    {
        //  Variables
        //  =========

        private readonly MainWindowViewModel mainWindowViewModel;

        //  Properties
        //  ==========

        public override bool IsClosable => mainWindowViewModel?.SelectedRepository?.URI == URI && mainWindowViewModel.Tabs.Count > 1;

        public override string URI => Models.URIs.URI.OpenRepository.Value;

        public override string Name => "Open";

        public override Command OnFocus { get; }

        public Command OpenNewRepository { get; }

        //  Constructors
        //  ============

        public OpenRepositoryViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;

            OnFocus = new Command(DoOnFocus);
            OpenNewRepository = new Command(DoOpenNewRepository);
        }

        //  Methods
        //  =======

        private void DoOnFocus()
        {

        }

        public void DoOpenNewRepository()
        {
            if (mainWindowViewModel == null)
            {
                throw new ArgumentNullException(nameof(mainWindowViewModel));
            }

            mainWindowViewModel.OpenNewRepositoryTab.DoExecute(null);
        }
    }
}
