using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kallithea_Klone
{
    public interface IState
    {
        void OnLoad();
        void OnLoaded();
        void OnLoseFocus();

        void OnSettings();
        void OnReload();

        void OnSearchTermChanged();
        void OnSearch();

        void OnMainAction();
    }
}
