using KallitheaKlone.Models.Repositories;
using KallitheaKlone.Models.Runner;
using KallitheaKlone.ViewModels;
using KallitheaKlone.WPF.Models.Repositories.Mercurial;
using KallitheaKlone.WPF.Models.Runner;
using System.Collections.ObjectModel;

namespace KallitheaKlone.WPF.ViewModels
{
    public class RepositoryMockViewModel : RepositoryViewModel
    {
        //  Constructors
        //  ============

        public RepositoryMockViewModel()
        {
            IRunner runner = new MockRunner();

            IChangeSet changeSet = new ChangeSet("1", "hash1", "author1", "timestamp1", "message1", runner);
            IChangeSet changeSet2 = new ChangeSet("2", "hash2", "author2", "timestamp2", "message2", runner);

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
            repository.ChangeSets.Add(changeSet2);

            repository.Branches.Add(new Branch("Branch 1", changeSet));
            repository.Branches.Add(new Branch("Branch 2", changeSet2));
            repository.Branches.Add(new Branch("Branch 3", changeSet2));

            repository.Tags.Add(new Tag("Tag 1", changeSet));
            repository.Tags.Add(new Tag("Tag 2", changeSet2));
            repository.Tags.Add(new Tag("Tag 3", changeSet2));

            RepositorySource = repository;
        }
    }
}
