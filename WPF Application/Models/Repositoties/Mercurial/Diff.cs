using KallitheaKlone.Models.Repositories;
using KallitheaKlone.Models.Runner;

namespace KallitheaKlone.WPF.Models.Repositoties.Mercurial
{
    public struct Diff : IDiff
    {
        //  Properties
        //  ==========

        public string Text { get; }

        //  Constructors
        //  ============

        public Diff(string text)
        {
            Text = text;
        }
    }
}