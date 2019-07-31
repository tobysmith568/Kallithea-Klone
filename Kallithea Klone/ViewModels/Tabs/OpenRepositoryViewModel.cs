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
        //  Properties
        //  ==========

        public override bool IsClosable => true;

        public override string URI => Models.URIs.URI.OpenRepository.Value;

        public override string Name => "Open";

        public override Command OnFocus { get; }

        //  Constructors
        //  ============

        public OpenRepositoryViewModel()
        {
            OnFocus = new Command(DoOnFocus);
        }

        //  Methods
        //  =======

        private void DoOnFocus()
        {

        }
    }
}
