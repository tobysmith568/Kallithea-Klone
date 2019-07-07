using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        ObservableCollection<IChangeSet> ChangeSets { get; }
        ObservableCollection<IBranch> Branches { get; }
        ObservableCollection<ITag> Tags { get; }

        IChangeSet SelectedChangeSet { get; set; }

        //  Methods
        //  =======

        /// <exception cref="VersionControlException"></exception>
        Task Load();
    }
}