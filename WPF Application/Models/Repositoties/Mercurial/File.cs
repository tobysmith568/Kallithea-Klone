﻿using KallitheaKlone.Models.Repositories;
using KallitheaKlone.Models.Runner;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KallitheaKlone.WPF.Models.Repositoties.Mercurial
{
    public class File : IFile
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

        public string Filename { get; }

        public ICollection<IDiff> Diffs { get; } = new List<IDiff>();

        public IChangeSet ChangeSet { get; }

        //  Constructors
        //  ============

        public File(IChangeSet changeSet, string filename, IRunner runner)
        {
            ChangeSet = changeSet ?? throw new ArgumentNullException(nameof(changeSet));
            Filename = filename ?? throw new ArgumentNullException(nameof(filename));

            this.runner = runner ?? throw new ArgumentNullException(nameof(runner));
        }

        //  Methods
        //  =======

        public async Task Load()
        {
            string command = $"hg diff -y --git -U 3 --noprefix --change {ChangeSet.Number} path:{Filename}";

            IRunResult runResult = await runner.Run(command);

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
                        Diffs.Add(new Diff(text));
                        text = string.Empty;
                    }
                    continue;
                }

                text += Environment.NewLine + line;
            }


        }
    }
}