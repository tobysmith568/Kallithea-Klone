using System.Windows;
using System.Windows.Controls;

namespace Kallithea_Klone
{
    public class CheckableItem : TreeViewItem
    {
        //  Properties
        //  ==========

        private Visibility _isChecked;

        public Visibility IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                foreach (var child in Items)
                {
                    if (((CheckableItem) child) != null)
                    {
                        ((CheckableItem)child).IsChecked = value;
                    }
                }
            }
        }

        //  Constructors
        //  ============

        public CheckableItem()
        {
            IsChecked = Visibility.Collapsed;
        }
    }
}
