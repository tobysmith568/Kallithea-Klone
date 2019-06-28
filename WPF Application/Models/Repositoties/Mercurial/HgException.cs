using KallitheaKlone.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.WPF.Models.Repositoties.Mercurial
{
    public class HgException : VersionControlException
    {
        //  Constructors
        //  ============

        public HgException(string command, string reason) : base(command, reason)
        {

        }
    }
}
