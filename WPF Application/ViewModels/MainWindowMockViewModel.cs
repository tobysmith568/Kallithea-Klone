using KallitheaKlone.Models.Repositories;
using KallitheaKlone.Models.Runner;
using KallitheaKlone.ViewModels;
using KallitheaKlone.WPF.Models.Repositoties.Mercurial;
using KallitheaKlone.WPF.Models.Runner;
using System.Collections.ObjectModel;

namespace KallitheaKlone.WPF.ViewModels
{
    public class MainWindowMockViewModel : MainWindowViewModel
    {
        //  Constructors
        //  ============

        public MainWindowMockViewModel()
        {
            IRunner runner = new Runner(@"D:\Users\Toby\Downloads\V21product");


            IChangeSet changeSet = new ChangeSet("2", runner);

            IFile file = new File(changeSet, "filename", runner);

            ObservableCollection<string> lines = new ObservableCollection<string>()
            {
                " This is a line",
                " This is a line",
                " This is a line",
                "+This is a line",
                " This is a line",
                " This is a line",
                "+This is a line",
                "-This is a line",
                " This is a line",
            };

            IDiff diff = new Diff(lines);

            file.Diffs.Add(diff);

            changeSet.Files.Add(file);

            Repository repository = new Repository(@"D:\Users\Toby\Downloads\V21product", "V22 Product");
            repository.ChangeSets.Add(changeSet);

            Repositories = new ObservableCollection<IRepository>()
            {
                repository
            };
        }
    }
}
