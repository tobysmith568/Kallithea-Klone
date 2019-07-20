using KallitheaKlone.Models.Repositories;
using KallitheaKlone.Models.Runner;
using System;

namespace KallitheaKlone.WPF.Models.Repositories.Mercurial
{
    public class Stash : IStash
    {
        //  Variables
        //  =========

        private readonly IRunner runner;

        //  Properties
        //  ==========

        public string Name { get; }

        //  Constructors
        //  ============

        public Stash(string name, IRunner runner)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            this.runner = runner ?? throw new ArgumentNullException(nameof(runner));
        }
    }
}
