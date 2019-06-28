namespace KallitheaKlone.Models.Repositories.RemoteRepositories
{
    public interface IRepository
    {
        //  Properties
        //  ==========

        string Name { get; set; }

        string URL { get; set; }

        RepositoryType RepositoryType { get; set; }
    }
}
