using System;
using System.Linq;
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

        public string Number { get; }
        public string Hash { get; }
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
        public string Message { get; }
        public string Author { get; }
        public string Timestamp { get; }
        public ICollection<IFile> Files { get; }

        //  Constructors
        //  ============

        private ChangeSet(string number, string hash, string author, string timestamp, string message, IRunner runner)
        {
            Number = number ?? throw new ArgumentNullException(nameof(number));
            Hash = hash ?? throw new ArgumentNullException(nameof(number));
            Author = author ?? throw new ArgumentNullException(nameof(author));
            Timestamp = timestamp ?? throw new ArgumentNullException(nameof(timestamp));
            Message = message ?? throw new ArgumentNullException(nameof(message));

            this.runner = runner ?? throw new ArgumentNullException(nameof(runner));
        }

        //  Methods
        //  =======

        /// <exception cref="HgException"></exception>
        public async static Task<IChangeSet> Load(string number, IRunner runner)
        {
            string command = "hg log -y --template \"{node}\n{author}\n{date|isodatesec}\n{desc}\" -r " + number;

            IRunResult runResult = await runner.Run(command);

            List<string> lines = runResult.StandardOut.ToList();

            if (lines.Count > 4)
            {
                throw new HgException(command, "did not return at least 4 lines");
            }

            IChangeSet result = new ChangeSet(number, lines[0], lines[1], lines[2], lines[3], runner);

            for (int i = 4; i < lines.Count; i++)
            {
                result.Files.Add(await File.Load(result, lines[i], runner));
            }

            return result;
        }
    }
}