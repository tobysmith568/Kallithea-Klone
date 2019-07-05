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
        ICollection<IChangeSet> ChangeSets { get; }
        ICollection<IBranch> Branches { get; }
        ICollection<ITag> Tags { get; }

        //  Methods
        //  =======

        /// <exception cref="VersionControlException"></exception>
        Task Load();
    }
}