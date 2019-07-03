using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.Models.Repositories
{
    public interface IRepository
    {
        //  Properties
        //  ==========

        RepositoryType RepositoryType { get; }
        string URI { get; set; }
        string Name { get; set; }

        //  Methods
        //  =======

        /// <exception cref="VersionControlException"></exception>
        Task<ICollection<IBranch>> GetAllBranches();

        IBranch GetCurrentBranch();

        ICollection<ITag> GetAllTags();

        /// <exception cref="VersionControlException"></exception>
        Task<ICollection<IChangeSet>> GetAllChangeSets();

        IChangeSet GetCurrentChangeSet();
    }
}