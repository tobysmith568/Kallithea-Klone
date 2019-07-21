namespace KallitheaKlone.Models.Repositories
{
    public interface IRepositoryFactory
    {
        //  Methods
        //  =======

        IRepository Create(RepositoryType repository, string location);
    }
}
