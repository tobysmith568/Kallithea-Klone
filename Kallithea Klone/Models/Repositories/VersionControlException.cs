using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.Models.Repositories
{
    public abstract class VersionControlException : Exception
    {
        //  Constants
        //  =========

        private const string Prefix = "The command [";
        private const string Middle = "] ";

        //  Constructors
        //  ============

        public VersionControlException(string command, string reason) : base(Prefix + command + Middle + reason)
        {

        }
    }
}