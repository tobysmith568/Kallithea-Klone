using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kallithea_Klone.States
{
    public interface IState
    {
        //  Properties
        //  ==========

        string Verb { get; }

        //  Methods
        //  =======

        void OnLoad();
        void OnLoaded();
        void OnLoseFocus();

        void OnSettings();
        void OnReload();

        void OnSearchTermChanged();
        void OnSearch();
        
        Task OnMainActionAsync(List<string> urls);
    }
}
