using KallitheaKlone.Enums;

namespace KallitheaKlone.Models.Repositories
{
    public abstract class RepositoryType : Enumeration
    {
        //  Properties
        //  ==========

        public string DataFolder { get; }

        //  Constructors
        //  ============

        protected RepositoryType(int id, string name, string dataFolder) : base(id, name)
        {
            DataFolder = dataFolder;
        }
    }
}
