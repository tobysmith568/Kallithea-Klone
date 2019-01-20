namespace Kallithea_Klone.ContextMenu
{
    public abstract class Child : IContextItem
    {
        //  Abstract Methods
        //  ================
        
        /// <exception cref="System.ObjectDisposedException"></exception>
        /// <exception cref="System.Security.SecurityException"></exception>
        /// <exception cref="System.UnauthorizedAccessException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public abstract string Create();
    }
}
