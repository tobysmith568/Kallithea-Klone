using KallitheaKlone.Models.Repositories;
using System;

namespace KallitheaKlone.WPF.Models.Repositories.Mercurial
{
    public class Tag : ITag
    {
        //  Properties
        //  ==========

        public string Name { get; }
        public IChangeSet ChangeSet { get; }

        //  Constructors
        //  ============

        public Tag(string name, IChangeSet changeSet)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ChangeSet = changeSet ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
