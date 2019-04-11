namespace Kallithea_Klone.Account_Settings
{
    public interface IAccountSettings
    {
        //  Properties
        //  ==========
        
        string _APIKey { get; set; }
        string _Username { get; set; }
        string _Host { get; set; }
        string _Password { get; set; }
        bool _Updates { get; set; }
        bool _JustInstalled { get; set; }
        AdvancedOptions _AdvancedOptions { get; set; }

        //  Methods
        //  =======

        void _Upgrade();
        void _Reset();
        void _Save();
        bool _VerifySettings();
    }
}