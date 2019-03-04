using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kallithea_Klone.WPF_Controls
{
    public class RepositoryFolder : TreeViewItem
    {
        //  Constructors
        //  ============

        public RepositoryFolder(string header) : base()
        {
            Header = header;
            FontSize = 18;
        }

        //  Methods
        //  =======

        /// <exception cref="InvalidOperationException"></exception>
        public RepositoryFolder GetOrAddChildFolder(string header)
        {
            RepositoryFolder result = Items.OfType<RepositoryFolder>().FirstOrDefault(c => c.Header.ToString() == header);

            if (result == null)
            {
                result = new RepositoryFolder(header);

                Items.Add(result);
            }

            return result;
        }

        /// <exception cref="InvalidOperationException"></exception>
        public Repository AddChildRepository(string name, string url, RoutedEventHandler clickEvent)
        {
            Repository result = new Repository(name, url, clickEvent);

            Items.Add(result);

            return result;
        }
    }
}
