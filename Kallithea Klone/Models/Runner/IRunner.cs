using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.Models.Runner
{
    public interface IRunner
    {
        //  Methods
        //  =======

        Task<IRunResult> Run(params string[] commands);
    }
}