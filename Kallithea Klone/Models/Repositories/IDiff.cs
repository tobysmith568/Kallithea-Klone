using System.Collections.ObjectModel;

namespace KallitheaKlone.Models.Repositories
{
    public interface IDiff
    {
        //  Properties
        //  ==========

        ObservableCollection<string> Text { get; }
    }
}