using KallitheaKlone.Models.Runner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.WPF.Models.Runner
{
    public class MockRunner : IRunner
    {
        public Task<IRunResult> Run(params string[] commands)
        {
            return Task.Run<IRunResult>(() =>
            {
                return new RunResult();
            });
        }
    }
}
