using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IntegrationTests : CategoryAttribute
    {
    }
}
