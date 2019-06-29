using KallitheaKlone.Models.Repositories;
using System;

namespace KallitheaKlone.WPF.Models.Repositoties.Mercurial
{
    public struct Branch : IBranch
    {
        //  Properties
        //  ==========

        public string Name { get; }
        public IChangeSet ChangeSet { get; }

        //  Constructors
        //  ============

        public Branch(string name, IChangeSet changeSet)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ChangeSet = changeSet ?? throw new ArgumentNullException(nameof(changeSet));
        }
    }
}