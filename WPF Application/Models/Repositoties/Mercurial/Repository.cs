using KallitheaKlone.Models.Repositories;
using KallitheaKlone.Models.Runner;
using System;
using System.Collections.Generic;
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

        private static readonly string[] lineEndings = { CarriageReturn, NewLine, BothNewLines};
        private static readonly char[] delimiters = { ' ', ':' };

        private int maximumChangesets = 100;

        //  Properties
        //  ==========

        public RepositoryType RepositoryType => RepositoryType.Mercurial;

        public string URI { get; set; }
        public string Name { get; set; }

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
        public async Task<ICollection<IBranch>> GetAllBranches()
        {
            string command = $"hg branches {StandardArgs} --template {AllBranchesTemplate}";

            IRunResult runResult = await runner.Run(command);

            List<string> lines = runResult.StandardOut.ToList();

            if (lines.Count % AllBranchesTemplateParts != 0)
            {
#warning TODO LOG
                throw new HgException(command, $"Returned [{lines.Count}] lines which isn't a multiple of [{AllBranchesTemplateParts}]");
            }

            ICollection<IBranch> results = new List<IBranch>();
            for (int i = 0; i < lines.Count; i += AllBranchesTemplateParts)
            {
                IChangeSet branchChangeSet = await ChangeSet.FromNumber(lines[i + 1], runner);
                results.Add(new Branch(lines[i], branchChangeSet));
            }

            return results;
        }

        public ICollection<ITag> GetAllTags()
        {
            throw new NotImplementedException();
        }

        public IBranch GetCurrentBranch()
        {
            throw new NotImplementedException();
        }

        /// <exception cref="VersionControlException"></exception>
        public async Task<ICollection<IChangeSet>> GetAllChangeSets()
        {
            string command = $"hg log {StandardArgs} --template {AllChangeSetsTemplate} -l {maximumChangesets}";

            IRunResult runResult = await runner.Run(command);

            List<string> lines = runResult.StandardOut.ToList();

            if (lines.Count % AllChangeSetsTemplateParts != 0)
            {
#warning TODO LOG
                throw new HgException(command, $"Returned [{lines.Count}] lines which isn't a multiple of [{AllChangeSetsTemplateParts}]");
            }

            ICollection<IChangeSet> result = new List<IChangeSet>();
            for (int i = 0; i < lines.Count; i += AllChangeSetsTemplateParts)
            {
                result.Add(new ChangeSet(lines[i], lines[i + 1], lines[i + 2], lines[i + 3], lines[i + 4], runner));
            }

            return result;
        }

        public IChangeSet GetCurrentChangeSet()
        {
            throw new NotImplementedException();
        }
    }
}