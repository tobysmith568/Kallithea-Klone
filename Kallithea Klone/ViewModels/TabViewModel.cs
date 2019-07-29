namespace KallitheaKlone.ViewModels
{
    public abstract class TabViewModel : ViewModelBase
    {
        //  Properties
        //  ==========

        public abstract bool IsClosable { get; }
        public abstract string URI { get; }
        public abstract string Name { get; }
    }
}