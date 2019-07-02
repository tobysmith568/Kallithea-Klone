using KallitheaKlone.Models.Repositories;
using KallitheaKlone.Models.Runner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        //  Variables
        //  =========

        private readonly IRunner runner;

        private readonly static string[] lineEndings = { CarriageReturn, NewLine, BothNewLines};
        private readonly static char[] delimiters = { ' ', ':' };

        //  Properties
        //  ==========

        public RepositoryType RepositoryType => RepositoryType.Mercurial;

        public string URI { get; set; }
        public string Name { get; set; }

        //  Constructor
        //  ===========

        public Repository(string uri, string name, IRunner runner)
        {
            URI = uri ?? throw new ArgumentNullException(nameof(uri));
            Name = name ?? throw new ArgumentNullException(nameof(name));

            this.runner = runner ?? throw new ArgumentNullException(nameof(runner));
        }

        //  Methods
        //  =======

        /// <exception cref="HgException"></exception>
        public async Task<ICollection<IBranch>> GetAllBranches()
        {
            IRunResult runResult = await runner.Run("hg branches -y");

            ICollection<IBranch> results = new List<IBranch>();

            foreach (string line in runResult.StandardOut)
            {
                string[] parts = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 3)
                {
                    //TODO LOG
                    continue;
                }

                IChangeSet branchChangeSet = await ChangeSet.Load(parts[1], runner);

                results.Add(new Branch(parts[0], branchChangeSet));
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

        public IChangeSet GetCurrentChangeSet()
        {
            throw new NotImplementedException();
        }
    }
}
