using System.Collections.Generic;
using System.Threading.Tasks;

namespace KallitheaKlone.Models.Repositories
{
    public interface IFile
    {
        //  Properties
        //  ==========

        string Filename { get; }
        ICollection<IDiff> Diffs { get; }

        //  Methods
        //  =======

        Task Load();
    }
}