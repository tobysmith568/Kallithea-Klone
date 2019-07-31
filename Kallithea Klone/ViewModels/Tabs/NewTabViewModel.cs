using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.ViewModels.Tabs
{
    public class NewTabViewModel : TabViewModel
    {
        //  Variables
        //  =========

        private readonly MainWindowViewModel mainWindowViewModel;

        //  Properties
        //  ==========

        public override bool IsClosable => false;

        public override string URI => Models.URIs.URI.NewTab.Value;

        public override string Name => "+";

        public override Command OnFocus { get; }

        //  Constructors
        //  ============

        /// <exception cref="InvalidOperationException"></exception>
        public NewTabViewModel(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel ?? throw new ArgumentNullException(nameof(mainWindowViewModel));

            OnFocus = new Command(DoOnFocus);
        }

        //  Methods
        //  =======

        /// <exception cref="InvalidOperationException"></exception>
        private void DoOnFocus()
        {
            mainWindowViewModel.OpenNewInternalTab.DoExecute(Models.URIs.URI.OpenRepository.Value);
        }
    }
}
