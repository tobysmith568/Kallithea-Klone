using System;
using System.Collections.Generic;
using System.IO;
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
        private static List<string> CheckedURLs = new List<string>();
        private static string RepoFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Kallithea Klone\\AllRepositories.dat";

        public MainWindow()
        {
            List<string> allRepositories = new List<string>();
            try
            {
                allRepositories = new List<string>(File.ReadAllLines(RepoFile));
            }
            catch
            {
                MessageBoxResult result = MessageBox.Show("Unable to read repositories!\t\t\t\t\nDo you want to re-load them?", "Critical error!", MessageBoxButton.YesNoCancel, MessageBoxImage.Error);
                switch (result)
                {
                    case MessageBoxResult.Cancel:
                        Environment.Exit(1);
                        break;
                    case MessageBoxResult.Yes:
                        break;
                    case MessageBoxResult.No:
                        break;
                    default:
                        break;
                }
            }

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

            InitializeComponent();

            //Create a treeview node for each location node
            foreach (Location location in baseLocation.InnerLocations)
            {
                CreateTreeViewItem(location);
            }
        }

        private void CreateTreeViewItem(Location location, TreeViewItem parent = null)
        {
            if (location.InnerLocations.Count == 0)
            {
                CheckBox newItem = new CheckBox
                {
                    Content = location.Name.Substring(0, location.Name.Length - 5),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Tag = parent.Tag + "/" + location.Name
                };
                newItem.Checked += NewItem_Checked;
                newItem.Unchecked += NewItem_Unchecked;

                if (parent == null)
                    MainTree.Items.Add(newItem);
                else
                    parent.Items.Add(newItem);
            }
            else
            {
                TreeViewItem newItem = new TreeViewItem
                {
                    Header = location.Name,
                };
                if (parent != null)
                    newItem.Tag = parent.Tag + "/" + location.Name;
                else
                    newItem.Tag = location.Name;

                if (parent == null)
                    MainTree.Items.Add(newItem);
                else
                    parent.Items.Add(newItem);

                foreach (Location subLocation in location.InnerLocations)
                    CreateTreeViewItem(subLocation, newItem);
            }

        }

        private void NewItem_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckedURLs.Remove(((Control)sender).Tag.ToString());
            lblNumberSelected.Content = CheckedURLs.Count + " " + (CheckedURLs.Count == 1 ? "Repository" : "Repositories") + " selected";
        }

        private void NewItem_Checked(object sender, RoutedEventArgs e)
        {
            CheckedURLs.Add(((Control)sender).Tag.ToString());
            lblNumberSelected.Content = CheckedURLs.Count + " " + (CheckedURLs.Count == 1 ? "Repository" : "Repositories") + " selected";
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

        private void btnClone_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Cloning:\n" + string.Join("\n", CheckedURLs.ToArray()));
        }

        private void BdrHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
