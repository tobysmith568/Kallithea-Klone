using KallitheaKlone.Models.Repositories;
using KallitheaKlone.Models.Runner;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KallitheaKlone.WPF.Models.Repositories.Mercurial
{
    public class Repository : IRepository
    {
        //  Constants
        //  =========

        private const string StandardArgs = "-y --pager no";

        private const string AllBranchesTemplate = "\"{branch}\\n{rev}\\n\"";
        private const int AllBranchesTemplateParts = 2;

        private const string AllTagsTemplate = "\"{tag}\\n{rev}\\n\"";
        private const int AllTagsTemplateParts = 2;

        private const string AllChangeSetsTemplate = "\"{rev}\\n{node}\\n{author}\\n{date|isodatesec}\\n{desc|firstline}\\n\"";
        private const int AllChangeSetsTemplateParts = 5;

        private const string AllowStash = "--config \"extensions.shelve = \"";
        private const string StashRegex = @"^[A-Za-z ]+?(?= *\([0-9]+?m ago\))";

        //  Variables
        //  =========

        private readonly IRunner runner;

        private readonly int maximumChangesets = 100;

        private readonly Regex stashRegex = new Regex(StashRegex);

        //  Properties
        //  ==========

        public RepositoryType RepositoryType => RepositoryType.Mercurial;

        public string URI { get; set; }
        public string Name { get; set; }
        public ObservableCollection<IChangeSet> ChangeSets { get; } = new ObservableCollection<IChangeSet>();
        public ObservableCollection<IBranch> Branches { get; private set; } = new ObservableCollection<IBranch>();
        public ObservableCollection<ITag> Tags { get; set; } = new ObservableCollection<ITag>();
        public ObservableCollection<IStash> Stashes { get; set; } = new ObservableCollection<IStash>();

        public IChangeSet SelectedChangeSet { get; set; }
        public IFile SelectedFile { get; set; }

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
            await LoadAllTags();
            await LoadAllStashes();
        }

        /// <exception cref="VersionControlException"></exception>
        private async Task LoadAllChangeSets()
        {
            string command = $"hg log {StandardArgs} --template {AllChangeSetsTemplate} -l {maximumChangesets}";

            IRunResult runResult = await runner.Run(command);

            List<string> lines = runResult.StandardOut.ToList();

            if (lines.Count % AllChangeSetsTemplateParts != 0)
            {
#warning //TODO LOG
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
#warning //TODO LOG
                throw new HgException(command, $"Returned [{lines.Count}] lines which isn't a multiple of [{AllBranchesTemplateParts}]");
            }

            for (int i = 0; i < lines.Count; i += AllBranchesTemplateParts)
            {
                IChangeSet branchChangeSet = new ChangeSet(lines[i + 1], runner);
                Branches.Add(new Branch(lines[i], branchChangeSet));
            }

            Branches = new ObservableCollection<IBranch>(Branches.OrderBy(b => b.Name));
        }

        /// <exception cref="VersionControlException"></exception>
        private async Task LoadAllTags()
        {
            string command = $"hg tags {StandardArgs} --template {AllTagsTemplate}";

            IRunResult runResult = await runner.Run(command);

            List<string> lines = runResult.StandardOut.ToList();

            if (lines.Count % AllTagsTemplateParts != 0)
            {
#warning //TODO LOG
                throw new HgException(command, $"Returned [{lines.Count}] lines which isn't a multiple of [{AllTagsTemplateParts}]");
            }

            for (int i = 0; i < lines.Count; i += AllTagsTemplateParts)
            {
                IChangeSet tagChangeSet = new ChangeSet(lines[i + 1], runner);
                Tags.Add(new Tag(lines[i], tagChangeSet));
            }

            Tags = new ObservableCollection<ITag>(Tags.OrderBy(t => t.Name));
        }

        /// <exception cref="VersionControlException"></exception>
        private async Task LoadAllStashes()
        {
            string command = $"hg {StandardArgs} {AllowStash} shelve --list";

            IRunResult runResult = await runner.Run(command);

            List<string> lines = runResult.StandardOut.ToList();

            for (int i = 0; i < lines.Count; i++)
            {
                try
                {
                    string name = stashRegex.Match(lines[i]).Value;
                    IStash stash = new Stash(name, runner);
                    Stashes.Add(stash);
                }
                catch (RegexMatchTimeoutException)
                {
#warning //TODO HANDLE
#warning //TODO LOG
                }
            }

            Stashes = new ObservableCollection<IStash>(Stashes.OrderBy(s => s.Name));
        }
    }
}