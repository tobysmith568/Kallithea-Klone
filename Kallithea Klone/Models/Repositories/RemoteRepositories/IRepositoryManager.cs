using System.Collections.Generic;
using System.Threading.Tasks;

namespace KallitheaKlone.Models.Repositories.RemoteRepositories
{
    public interface IRepositoryManager<F, R> where F : IRepositoryFolder<F, R> where R : IRepository
    {
        //  Methods
        //  =======

        Task<IRepositoryFolder<F, R>> GetAllRepositories();

        Task<bool> OverwriteAllRespositories(IRepositoryFolder<F, R> repositories);
    }
}
