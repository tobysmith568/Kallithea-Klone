using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.ViewModels
{
    public class NewTabViewModel : TabViewModel
    {
        public override bool IsClosable => false;

        public override string URI => "internal://NewTab";

        public override string Name => "+";
    }
}
