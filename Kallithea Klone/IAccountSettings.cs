namespace Kallithea_Klone
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
    }
}