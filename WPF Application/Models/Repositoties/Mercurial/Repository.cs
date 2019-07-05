using KallitheaKlone.Models.Repositories;
using KallitheaKlone.Models.Runner;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace KallitheaKlone.WPF.Models.Repositoties.Mercurial
{
    public class Repository : IRepository
    {
        //  Constants
        //  =========

        private const string CarriageReturn = "\r";
        private const string NewLine = "\n";
        private const string BothNewLines = CarriageReturn + NewLine;

        private const string StandardArgs = "-y --pager no";

        private const string AllBranchesTemplate = "\"{branch}\\n{rev}\\n\"";
        private const int AllBranchesTemplateParts = 2;

        private const string AllChangeSetsTemplate = "\"{rev}\\n{node}\\n{author}\\n{date|isodatesec}\\n{desc|firstline}\\n\"";
        private const int AllChangeSetsTemplateParts = 5;

        //  Variables
        //  =========

        private readonly IRunner runner;

        private int maximumChangesets = 100;

        //  Properties
        //  ==========

        public RepositoryType RepositoryType => RepositoryType.Mercurial;

        public string URI { get; set; }
        public string Name { get; set; }
        public ICollection<IChangeSet> ChangeSets { get; } = new List<IChangeSet>();
        public ObservableCollection <IBranch> Branches { get; } = new ObservableCollection<IBranch>();
        public ICollection<ITag> Tags { get; } = new List<ITag>();

        //  Constructor
        //  ===========

        public Repository(string uri, string name)
        {
            URI = uri ?? throw new ArgumentNullException(nameof(uri));
            Name = name ?? throw new ArgumentNullException(nameof(name));

            runner = new Runner.Runner(uri);
        }

        //  Methods
        //  =======

        /// <exception cref="VersionControlException"></exception>
        public async Task Load()
        {
            await LoadAllChangeSets();
            await LoadAllBranches();
#warning TODO await LoadAllTags();
        }

        /// <exception cref="VersionControlException"></exception>
        private async Task LoadAllChangeSets()
        {
            string command = $"hg log {StandardArgs} --template {AllChangeSetsTemplate} -l {maximumChangesets}";

            IRunResult runResult = await runner.Run(command);

            List<string> lines = runResult.StandardOut.ToList();

            if (lines.Count % AllChangeSetsTemplateParts != 0)
            {
#warning TODO LOG
                throw new HgException(command, $"Returned [{lines.Count}] lines which isn't a multiple of [{AllChangeSetsTemplateParts}]");
            }

            for (int i = 0; i < lines.Count; i += AllChangeSetsTemplateParts)
            {
                ChangeSets.Add(new ChangeSet(lines[i], lines[i + 1], lines[i + 2], lines[i + 3], lines[i + 4], runner));
            }
        }

        /// <exception cref="VersionControlException"></exception>
        private async Task LoadAllBranches()
        {
            string command = $"hg branches {StandardArgs} --template {AllBranchesTemplate}";

            IRunResult runResult = await runner.Run(command);

            List<string> lines = runResult.StandardOut.ToList();

            if (lines.Count % AllBranchesTemplateParts != 0)
            {
#warning TODO LOG
                throw new HgException(command, $"Returned [{lines.Count}] lines which isn't a multiple of [{AllBranchesTemplateParts}]");
            }

            for (int i = 0; i < lines.Count; i += AllBranchesTemplateParts)
            {
                IChangeSet branchChangeSet = new ChangeSet(lines[i + 1], runner);
                Branches.Add(new Branch(lines[i], branchChangeSet));
            }
        }

        private async Task LoadAllTags()
        {
            throw new NotImplementedException();
        }
    }
}