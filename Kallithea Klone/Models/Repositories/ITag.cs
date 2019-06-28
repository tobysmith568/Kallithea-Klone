namespace KallitheaKlone.Models.Repositories
{
    public interface ITag
    {
        //  Properties
        //  ==========

        string Name { get; set; }
        IChangeSet ChangeSet { get; set; }
    }
}