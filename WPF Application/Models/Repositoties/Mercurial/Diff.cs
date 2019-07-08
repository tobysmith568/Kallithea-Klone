using KallitheaKlone.Models.Repositories;
using System.Collections.ObjectModel;

namespace KallitheaKlone.WPF.Models.Repositoties.Mercurial
{
    public class Diff : IDiff
    {
        //  Properties
        //  ==========

        public ObservableCollection<string> Text { get; }

        //  Constructors
        //  ============

        public Diff(ObservableCollection<string> text)
        {
            Text = text;
        }

        public Diff() : this(new ObservableCollection<string>())
        {
        }
    }
}