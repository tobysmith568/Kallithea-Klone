using KallitheaKlone.Models.Runner;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace KallitheaKlone.Models.Repositories
{
    public interface IChangeSet
    {
        //  Properties
        //  ==========

        string Number { get; }
        string Hash { get; }
        string ShortHash { get; }
        string Message { get; }
        string Author { get; }
        string Timestamp { get; }
        ObservableCollection<IFile> Files { get; }

        //  Methods
        //  =======

        Task Load();
    }
}   