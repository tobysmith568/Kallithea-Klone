using KallitheaKlone.Enums;
using KallitheaKlone.Models.Repositories;
using System;

namespace KallitheaKlone.WFP.Models.Repositories
{
    public class WPFRepositoryType : RepositoryType
    {
        //  Variables
        //  =========

        public static WPFRepositoryType Git = new WPFRepositoryType(1, "Git", ".git");
        public static WPFRepositoryType Mercurial = new WPFRepositoryType(2, "Mercurial", ".hg");

        //  Constructors
        //  ============

        private WPFRepositoryType(int id, string name, string dataFolder) : base(id, name, dataFolder)
        {
        }
    }
}
