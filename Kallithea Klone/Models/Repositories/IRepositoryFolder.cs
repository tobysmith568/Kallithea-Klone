using System.Collections.Generic;

namespace KallitheaKlone.Models.Repositories
{
    public interface IRepositoryFolder<F, R> where F : IRepositoryFolder<F, R> where R : IRepository
    {
        //  Properties
        //  ==========

        string Name { get; set; }

        ICollection<F> ChildFolders { get; set; }

        ICollection<R> ChildRepositories { get; set; }
    }
}
