using KallitheaKlone.Models.Dialogs.MessagePrompts;
using KallitheaKlone.Models.JSONConverter;
using KallitheaKlone.Models.RemoteRepositories;
using KallitheaKlone.WPF.Models.Dialogs.MessagePrompts;
using KallitheaKlone.WPF.Models.JSONConverter;
using KallitheaKlone.WPF.Models.RemoteRepositories;
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
using System.Windows.Shapes;

namespace KallitheaKlone.WPF.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            /*
            IRepositoryFolder @base = new RepositoryFolder
            {
                ChildFolders = new List<IRepositoryFolder>
                {
                    new RepositoryFolder
                    {
                        ChildFolders = new List<IRepositoryFolder>
                        {
                            
                        },
                        ChildRepositories = new List<IRepository>
                        {
                            new Repository()
                            {
                                Name = "Somthing",
                                URL = "URL"
                            }
                        },
                        Name = "AA"
                    },
                    new RepositoryFolder
                    {
                        ChildFolders = new List<IRepositoryFolder>
                        {

                        },
                        ChildRepositories = new List<IRepository>
                        {

                        },
                        Name = "AB"
                    }
                },
                ChildRepositories = new List<IRepository>
                {
                    new Repository()
                    {
                        Name = "Somthing Else",
                        URL = "URL also"
                    }
                },
                Name = "A"
            };

            IMessagePrompt messagePrompt = new MessagePrompt();
            IJSONConverter jsonConverter = new NewtonSoftJSONConverter();
            IRepositoryManager manager = new RepositoryManager(messagePrompt, jsonConverter);

            manager.OverwriteAllRespositories(@base);*/
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
           IMessagePrompt messagePrompt = new MessagePrompt();
            IJSONConverter jsonConverter = new NewtonSoftJSONConverter();
            IRepositoryManager<RepositoryFolder, Repository> manager = new RepositoryManager(messagePrompt, jsonConverter);


            IRepositoryFolder<RepositoryFolder, Repository> result = await manager.GetAllRepositories();
            Console.WriteLine("");
        }
    }
}
