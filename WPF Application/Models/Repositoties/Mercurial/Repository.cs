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

        public Repository(IRunner runner)
        {
            this.runner = runner ?? throw new ArgumentNullException(nameof(runner));
        }

        //  Methods
        //  =======

        /// <exception cref="HgException"></exception>
        public async Task<ICollection<IBranch>> GetAllBranches()
        {
            IRunResult runResult = await runner.Run("hg branches -y");

            ICollection<IBranch> results = new List<IBranch>();

            foreach (string line in runResult.StandardOut.Split(lineEndings, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] parts = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 3)
                {
                    //TODO LOG
                    continue;
                }

                results.Add(new Branch()
                {
                    Name = parts[0],
                    ChangeSet = await ChangeSet.Load(parts[1], runner)
                });
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
