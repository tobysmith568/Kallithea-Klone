using KallitheaKlone.Models.Repositories;

namespace KallitheaKlone.WPF.Models.Repositoties.Mercurial
{
    public class Branch : IBranch
    {
        //  Properties
        //  ==========

        public string Name { get; set; }
        public IChangeSet ChangeSet { get; set; }
    }
}