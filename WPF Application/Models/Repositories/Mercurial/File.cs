﻿using KallitheaKlone.Models.Repositories;
using KallitheaKlone.Models.Runner;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace KallitheaKlone.WPF.Models.Repositories.Mercurial
{
    public class File : IFile
    {
        //  Variables
        //  =========

        private readonly IRunner runner;

        //  Properties
        //  ==========

        public string Filename { get; }

        public ObservableCollection<IDiff> Diffs { get; } = new ObservableCollection<IDiff>();

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

            Diff diff = null;
            foreach (string line in runResult.StandardOut)
            {
                if (line.StartsWith("diff")
                 || line.StartsWith("+++")
                 || line.StartsWith("---"))
                {
                    continue;
                }

                if (line.StartsWith("@@"))
                {
                    if (diff != null)
                    {
                        Diffs.Add(diff);
                        diff = new Diff();
                    }
                    continue;
                }

                if (diff == null)
                {
                    diff = new Diff();
                }
                diff.Text.Add(line);
            }

            if (diff != null)
            {
                Diffs.Add(diff);
            }
        }
    }
}