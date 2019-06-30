using System.Collections.Generic;

namespace KallitheaKlone.Models.Repositories
{
    public interface IFile
    {
        //  Properties
        //  ==========

        string Filename { get; }

        ICollection<IDiff> Diffs { get; }
    }
}