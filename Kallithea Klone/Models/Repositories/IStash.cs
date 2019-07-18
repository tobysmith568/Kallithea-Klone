using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace KallitheaKlone.Models.Repositories
{
    public interface IStash
    {
        //  Properties
        //  ==========

        string Name { get; }
#warning  //TODO        ObservableCollection<IDiff> Diffs { get; }

        //  Methods
        //  =======

#warning  //TODO        Task Load();
    }
}