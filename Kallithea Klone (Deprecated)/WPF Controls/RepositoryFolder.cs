using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kallithea_Klone.WPF_Controls
{
    public class RepositoryFolder : TreeViewItem, IRepoControl
    {
        //  Properties
        //  ==========

        public SortingCategory SortingCategory { get; } = SortingCategory.Folder;
        public string LocationName { get; }

        //  Constructors
        //  ============

        public RepositoryFolder(string header) : base()
        {
            LocationName = header;
            Header = header;
            FontSize = 18;
        }

        //  Methods
        //  =======
        
        public RepositoryFolder GetOrAddChildFolder(string header)
        {
            RepositoryFolder result = Items.OfType<RepositoryFolder>().FirstOrDefault(c => c.Header.ToString() == header);

            if (result == null)
            {
                result = new RepositoryFolder(header);
                ItemsSource = AddNewItemInOrder(Items.OfType<IRepoControl>().ToList(), result);
            }

            return result;
        }

        public Repository AddChildRepository(string name, string url, RoutedEventHandler clickEvent)
        {
            Repository result = new Repository(name, url, clickEvent);

            ItemsSource = AddNewItemInOrder(Items.OfType<IRepoControl>().ToList(), result);

            return result;
        }

        private ICollection<IRepoControl> AddNewItemInOrder(ICollection<IRepoControl> collection, IRepoControl newItem)
        {
            collection.Add(newItem);

            collection = collection.OrderBy(l => l.SortingCategory).ThenBy(l => l.LocationName).ToList();

            return collection;
        }
    }
}
