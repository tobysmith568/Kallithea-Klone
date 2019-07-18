namespace KallitheaKlone.Models.Repositories
{
    public interface ITag
    {
        //  Properties
        //  ==========

        string Name { get; }
        IChangeSet ChangeSet { get; }
    }
}