using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KallitheaKlone.Models.Repositories;
using KallitheaKlone.Models.Runner;

namespace KallitheaKlone.WPF.Models.Repositoties.Mercurial
{
    public class ChangeSet : IChangeSet
    {
        //  Constants
        //  =========

        private const string CarriageReturn = "\r";
        private const string NewLine = "\n";
        private const string BothNewLines = CarriageReturn + NewLine;

        //  Variables
        //  =========

        private IRunner runner;

        private readonly static string[] lineEndings = { CarriageReturn, NewLine, BothNewLines };

        //  Properties
        //  ==========

        public string Number { get; set; }
        public string Hash { get; set; }
        public string ShortHash
        {
            get
            {
                if (Hash == null)
                {
                    return null;
                }

                if (Hash.Length <= 12)
                {
                    return Hash;
                }

                return Hash.Substring(0, 12);
            }
        }
        public string Message { get; set; }
        public string Author { get; set; }
        public string Timestamp { get; set; }
        public ICollection<IDiff> Diffs { get; set; }

        //  Constructors
        //  ============

        private ChangeSet(string number, IRunner runner)
        {
            Number = number ?? throw new ArgumentNullException(nameof(number));
            this.runner = runner ?? throw new ArgumentNullException(nameof(runner));
        }

        //  Methods
        //  =======

        /// <exception cref="HgException"></exception>
        public async static Task<IChangeSet> Load(string number, IRunner runner)
        {
            string command = "hg log -y --template \"{node}\n{author}\n{date|isodatesec}\n{desc}\" -r " + number;

            IRunResult runResult = await runner.Run(command);

            string[] lines = runResult.StandardOut.Split(lineEndings, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length > 4)
            {
                throw new HgException(command, "did not return at least 4 lines");
            }

            IChangeSet result = new ChangeSet(number, runner)
            {
                Hash = lines[0],
                Author = lines[1],
                Timestamp = lines[2],
                Message = lines[3]
            };

            for (int i = 4; i < lines.Length; i++)
            {
                result.Files.Add(new File(lines[i]));
            }

            return result;
        }
    }
}