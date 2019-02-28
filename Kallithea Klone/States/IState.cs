using Kallithea_Klone.Other_Classes;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Kallithea_Klone.States
{
    public interface IState
    {
        //  Properties
        //  ==========

        string Verb { get; }
        string RunLocation { get; }

        //  Methods
        //  =======

        void InitialActions(string[] args);
        /// <exception cref="InvalidOperationException"></exception>
        ICollection<Location> OnLoadRepositories();
        MainWindowStartProperties OnLoaded();
        void OnLoseFocus(bool completingMainAction);

        void OnSettings();
        Task<ICollection<Location>> OnReloadAsync();
        
        Task OnMainActionAsync(List<Location> locations);
    }
}
