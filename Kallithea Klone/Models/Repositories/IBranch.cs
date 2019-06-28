namespace KallitheaKlone.Models.Repositories
{
    public interface IBranch
    {
        //  Properties
        //  ==========

        string Name { get; set; }
        IChangeSet ChangeSet { get; set; }
    }
}