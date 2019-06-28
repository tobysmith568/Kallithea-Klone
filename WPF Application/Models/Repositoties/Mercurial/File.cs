using KallitheaKlone.Models.Repositories;

namespace KallitheaKlone.WPF.Models.Repositoties.Mercurial
{
    internal class File : IFile
    {
        //  Properties
        //  ==========

        public string Filename { get; set; }

        //  Constructors
        //  ============

        public File(string filename)
        {
            Filename = filename;
        }
    }
}