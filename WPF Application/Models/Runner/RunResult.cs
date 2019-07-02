using KallitheaKlone.Models.Runner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.WPF.Models.Runner
{
    public class RunResult : IRunResult
    {
        //  Properties
        //  ==========

        public int ExitCode { get; set; }
        public ICollection<string> AllOut { get; set; }
        public ICollection<string> StandardOut { get; set; }
        public ICollection<string> ErrorOut { get; set; }
    }
}
