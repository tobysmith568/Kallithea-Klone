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
        ICollection<string> OnLoadRepositories();
        MainWindowStartProperties OnLoaded();
        void OnLoseFocus(bool completingMainAction);

        void OnSettings();
        Task<ICollection<string>> OnReloadAsync();
        
        Task OnMainActionAsync(ICollection<RepositoryData> locations);
    }
}
