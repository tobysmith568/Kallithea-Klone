namespace Kallithea_Klone.States
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
