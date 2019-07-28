using KallitheaKlone.Models.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallitheaKlone.ViewModels
{
    public class RepositoryViewModel : TabViewModel
    {
        //  Properties
        //  ==========

        public override string URI
        {
            get => RepositorySource?.URI ?? string.Empty;
        }

        public override string Name
        {
            get => Path.GetFileName(URI);
        }

        public IRepository RepositorySource { get; set; }
        public Command<IChangeSet> LoadSelectedChangeSet { get; }

        //  Constructor
        //  ===========

        public RepositoryViewModel()
        {
            LoadSelectedChangeSet = new Command<IChangeSet>(DoLoadSelectedChangeSet);
        }

        //  Methods
        //  =======

        private async void DoLoadSelectedChangeSet(IChangeSet changeSet)
        {
            await changeSet.Load();

            foreach (IFile file in changeSet.Files)
            {
                await file.Load();
            }
        }
    }
}
