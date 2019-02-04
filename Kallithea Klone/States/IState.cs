using Kallithea_Klone.Other_Classes;
using System.Collections;
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

        /// <exception cref="InvalidOperationException"></exception>
        ICollection<Control> OnLoadRepositories();
        MainWindowStartProperties OnLoaded();
        void OnLoseFocus();

        void OnSettings();
        void OnReload();

        ICollection<Control> OnSearchCleared(string searchTerm);
        ICollection<Control> OnSearch(string searchTerm);
        
        Task OnMainActionAsync(List<string> urls);
    }
}
