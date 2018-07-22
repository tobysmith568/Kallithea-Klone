using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kallithea_Klone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List<string> allRepositories = new List<string>
            {
                "VCM/Krispy Kreme/VCM001.pull",
                "VCM/Krispy Kreme/VCM002.pull",
                "VCM/Krispy Kreme/VCM003.pull",
                "VCM/Krispy Kreme/VCM004.pull",
                "VCM/Lego/VCM001.pull",
                "VCM/Lego/VCM002.pull",
                "VCM/Lego/VCM003.pull",
                "VCM/Jack Wills/VCM001.pull",
                "VCM/Jack Wills/VCM002.pull",
                "VCM/Jack Wills/VCM003.pull",
                "VCM/Jack Wills/VCM004.pull",
                "VCM/Jack Wills/VCM005.pull",
                "VCM/Jack Wills/VCM006.pull",
                "VCM/Jack Wills/VCM007.pull",
                "Something/Jack Wills/JW 1.pull",
                "Something/Jack Wills/JW 2.pull",
            };

            //Create tree of menu items
            Location baseLocation = new Location("Base Location");

            string[] parts;
            foreach (string location in allRepositories)
            {
                parts = location.Split('/');
                Location current = baseLocation;
                for (int i = 0; i < parts.Length; i++)
                {
                    current = GetOrCreate(current, parts[i]);
                }
            }

            //Create a treeview node for each location node
            foreach (Location location in baseLocation.InnerLocations)
            {
                CreateTreeViewItem(location);
            }
        }

        private void CreateTreeViewItem(Location location, TreeViewItem parent = null)
        {
            TreeViewItem newItem = new TreeViewItem();
            newItem.Header = location.Name;

            if (parent == null)
                MainTree.Items.Add(newItem);
            else
                parent.Items.Add(newItem);

            foreach (Location subLocation in location.InnerLocations)
                CreateTreeViewItem(subLocation, newItem);

        }

        private static Location GetOrCreate(Location current, string location)
        {
            if (current.InnerLocations.Count(l => l.Name == location) > 0)
                return current.InnerLocations.FirstOrDefault(l => l.Name == location);
            else
            {
                Location inner = new Location
                {
                    Name = location
                };
                current.InnerLocations.Add(inner);
                return inner;
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
