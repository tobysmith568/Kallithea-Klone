using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace KallitheaKlone.Models.Repositories
{
    public interface IFile
    {
        //  Properties
        //  ==========

        string Filename { get; }
        ObservableCollection<IDiff> Diffs { get; }

        //  Methods
        //  =======

        Task Load();
    }
}