using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kallithea_Klone.Kallithea_API
{
    class InvalidKallitheaResponseTypeException : Exception
    {
        public InvalidKallitheaResponseTypeException()
        {
        }

        public InvalidKallitheaResponseTypeException(Type t) : base($"The given response does not match the type: {t.Name}")
        {
        }

        public InvalidKallitheaResponseTypeException(string message) : base(message)
        {
        }

        public InvalidKallitheaResponseTypeException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
