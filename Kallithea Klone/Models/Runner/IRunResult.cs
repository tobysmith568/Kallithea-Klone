using System.Collections.Generic;

namespace KallitheaKlone.Models.Runner
{
    public interface IRunResult
    {
        //  Properties
        //  ==========

        int ExitCode { get; set; }
        ICollection<string> AllOut { get; set; }
        ICollection<string> StandardOut { get; set; }
        ICollection<string> ErrorOut { get; set; }
    }
}