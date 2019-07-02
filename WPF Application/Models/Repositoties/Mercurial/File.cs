using KallitheaKlone.Models.Repositories;
using KallitheaKlone.Models.Runner;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KallitheaKlone.WPF.Models.Repositoties.Mercurial
{
    public struct File : IFile
    {
        //  Constants
        //  =========

        private const string CarriageReturn = "\r";
        private const string NewLine = "\n";
        private const string BothNewLines = CarriageReturn + NewLine;

        //  Variables
        //  =========

        private readonly static string[] lineEndings = { CarriageReturn, NewLine, BothNewLines };

        //  Properties
        //  ==========

        public string Filename { get; }

        public ICollection<IDiff> Diffs { get; }

        //  Constructors
        //  ============

        private File(string filename, ICollection<IDiff> diffs)
        {
            Filename = filename ?? throw new ArgumentNullException(nameof(filename));
            Diffs = diffs ?? throw new ArgumentNullException(nameof(diffs));
        }

        //  Methods
        //  =======

        public static async Task<IFile> Load(IChangeSet changeSet, string filename, IRunner runner)
        {
            changeSet = changeSet ?? throw new ArgumentNullException(nameof(changeSet));
            runner = runner ?? throw new ArgumentNullException(nameof(runner));

            string command = $"hg diff -y --git -U 3 --noprefix --change {changeSet.Number} path:{filename}";

            IRunResult runResult = await runner.Run(command);

            ICollection<IDiff> diffs = new List<IDiff>();
            string text = string.Empty;
            foreach (string line in runResult.StandardOut)
            {
                if (!line.StartsWith(string.Empty)
                 || !line.StartsWith("diff")
                 || !line.StartsWith("+++")
                 || !line.StartsWith("---"))
                {
                    continue;
                }

                if (line.StartsWith("@@"))
                {
                    if (text != string.Empty)
                    {
                        diffs.Add(new Diff(text));
                        text = string.Empty;
                    }
                    continue;
                }

                text += Environment.NewLine + line;
            }

            return new File(filename, diffs);
        }
    }
}