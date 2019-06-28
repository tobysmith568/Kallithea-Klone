using System.Collections.Generic;

namespace KallitheaKlone.Models.Repositories
{
    public interface IChangeSet
    {
        //  Properties
        //  ==========

        string Number { get; set; }
        string Hash { get; set; }
        string ShortHash { get; }
        string Message { get; set; }
        string Author { get; set; }
        string Timestamp { get; set; }
        ICollection<IFile> Files { get; set; }
    }
}   