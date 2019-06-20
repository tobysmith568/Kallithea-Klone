using System.Threading.Tasks;

namespace KallitheaKlone.Models.Updater
{
    public interface IUpdater
    {
        //  Methods
        //  =======

        Task<Asset> CheckForUpdateAsync();
        void UpdateTo(Asset asset);
    }
}
