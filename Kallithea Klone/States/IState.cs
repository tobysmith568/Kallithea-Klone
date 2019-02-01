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

        //  Methods
        //  =======

        ICollection<Control> OnLoadRepositories();
        void OnLoaded();
        void OnLoseFocus();

        void OnSettings();
        void OnReload();

        ICollection<Control> OnSearchCleared(string searchTerm);
        ICollection<Control> OnSearch(string searchTerm);
        
        Task OnMainActionAsync(string localLocation, List<string> urls);
    }
}
