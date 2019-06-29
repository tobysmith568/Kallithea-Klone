namespace KallitheaKlone.Models.Repositories
{
    public interface IBranch
    {
        //  Properties
        //  ==========

        string Name { get; }
        IChangeSet ChangeSet { get; }
    }
}