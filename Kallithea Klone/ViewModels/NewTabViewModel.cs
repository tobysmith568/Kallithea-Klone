using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.ViewModels
{
    public class NewTabViewModel : TabViewModel
    {
        //  Constants
        //  =========

        private const string uri = "internal://NewTab";
        private const string name = "+";

        //  Properties
        //  ==========

        public override bool IsClosable => false;

        public override string URI => uri;

        public override string Name => name;

        public override Command OnFocus { get; }

        //  Constructors
        //  ============

        public NewTabViewModel()
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
