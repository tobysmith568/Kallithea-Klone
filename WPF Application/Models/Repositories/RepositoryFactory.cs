using KallitheaKlone.Models.Repositories;
using KallitheaKlone.WFP.Models.Repositories;
using System;
using System.IO;

namespace KallitheaKlone.WPF.Models.Repositories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        //  Methods
        //  =======

        public IRepository Create(RepositoryType repository, string location)
        {
            if (repository == WPFRepositoryType.Git)
            {
                throw new NotImplementedException();
            }

            if (repository == WPFRepositoryType.Mercurial)
            {
                return new Mercurial.Repository(location, Path.GetFileName(location));
            }

            throw new NotImplementedException();
        }
    }
}
