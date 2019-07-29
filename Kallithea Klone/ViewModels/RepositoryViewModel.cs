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

        public override bool IsClosable => true;

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
        public override Command OnFocus { get; }

        //  Constructor
        //  ===========

        public RepositoryViewModel()
        {
            LoadSelectedChangeSet = new Command<IChangeSet>(DoLoadSelectedChangeSet);
            OnFocus = new Command(DoOnFocus);
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

        private void DoOnFocus()
        {
            Console.WriteLine();
        }
    }
}
