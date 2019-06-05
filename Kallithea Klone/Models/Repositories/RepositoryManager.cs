using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.Models.Repositories
{
    public class RepositoryManager : IRepositoryManager<Repository>
    {
        public Task<Repository> GetAllRepositories()
        {
            throw new NotImplementedException();
        }

        public Task<bool> OverwriteAllRespositories(ICollection<Repository> repositories)
        {
            throw new NotImplementedException();
        }
    }
}
