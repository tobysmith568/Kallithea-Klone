namespace KallitheaKlone.Models.Settings
{
    public interface ISettings
    {
        //  Properties
        //  ==========

        bool JustInstalled { get; set; }

        //  Methods
        //  =======

        void Upgrade();
        void SaveAll();
    }
}
