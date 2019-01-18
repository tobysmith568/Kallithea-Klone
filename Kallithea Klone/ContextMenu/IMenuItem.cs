namespace Kallithea_Klone.ContextMenu
{
    public interface IMenuItem : IContextItem
    {
        //  Properties
        //  ==========

        string Name { get; set; }
        string Label { get; set; }
        string Icon { get; set; }
        string Command { get; set; }
    }
}
