using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Kallithea_Klone.WPF_Controls
{
    public class RepositoryFolder : TreeViewItem
    {
        //  Constructors
        //  ============

        public RepositoryFolder(string header) : base()
        {
            Header = header;
            FontSize = 18;
        }
    }
}
