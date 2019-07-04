using KallitheaKlone.Models.Runner;
using System.Collections.Generic;
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
        ICollection<IFile> Files { get; }

        //  Methods
        //  =======

        Task Load();
    }
}   