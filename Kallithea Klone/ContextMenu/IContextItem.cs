namespace Kallithea_Klone.ContextMenu
{
    public interface IContextItem
    {
        //  Methods
        //  =======
        
        /// <exception cref="System.ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        string Create();
    }
}
