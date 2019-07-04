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

        private const string StandardArgs = "-y --pager no";

        private const string ChangeSetFromNumberTemplate = "\"{node}\\n{author}\\n{date|isodatesec}\\n{desc}\\n\"";
        private const int ChangeSetFromNumberTemplateParts = 4;

        //  Variables
        //  =========

        private IRunner runner;

        private readonly static string[] lineEndings = { CarriageReturn, NewLine, BothNewLines };

        //  Properties
        //  ==========

        public string Number { get; }
        public string Hash { get; private set; }
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
        public string Message { get; private set; }
        public string Author { get; private set; }
        public string Timestamp { get; private set; }
        public ICollection<IFile> Files { get; private set; } = new List<IFile>();

        //  Constructors
        //  ============

        public ChangeSet(string number, IRunner runner)
        {
            Number = number ?? throw new ArgumentNullException(nameof(number));

            this.runner = runner ?? throw new ArgumentNullException(nameof(runner));
        }

        public ChangeSet(string number, string hash, string author, string timestamp, string message, IRunner runner)
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
        public async Task Load()
        {
            string command = $"hg log {StandardArgs} --template {ChangeSetFromNumberTemplate} -r {Number}" +
                            $"&hg status {StandardArgs} --change {Number} -n";

            IRunResult runResult = await runner.Run(command);

            List<string> lines = runResult.StandardOut.ToList();

            if (lines.Count < ChangeSetFromNumberTemplateParts)
            {
                throw new HgException(command, $"did not return at least {ChangeSetFromNumberTemplateParts} lines");
            }

            Hash = lines[0];
            Author = lines[1];
            Timestamp = lines[2];
            Message = lines[3];

            for (int i = ChangeSetFromNumberTemplateParts; i < lines.Count; i++)
            {
                Files.Add(new File(this, lines[i], runner));
            }
        }
    }
}