using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.Models.Repositories
{
    public interface IRepositoryManager<T> where T : IRepository
    {
        //  Methods
        //  =======

        Task<ICollection<T>> GetAllRepositories();

        Task<bool> OverwriteAllRespositories(ICollection<T> repositories);
    }
}
