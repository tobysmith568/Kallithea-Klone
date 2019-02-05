using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Kallithea_Klone.Other_Classes;

namespace Kallithea_Klone.States
{
    public abstract class LogicOnlyState : IState
    {
        //  Abstract State Methods
        //  ======================

        public abstract void InitialActions();

        //  State Methods
        //  =============

        public string Verb => throw new InvalidOperationException();

        public string RunLocation => throw new InvalidOperationException();

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        public MainWindowStartProperties OnLoaded()
        {
            throw new InvalidOperationException();
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        public ICollection<Control> OnLoadRepositories()
        {
            throw new InvalidOperationException();
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        public void OnLoseFocus(bool completingMainAction)
        {
            throw new InvalidOperationException();
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        public Task OnMainActionAsync(List<string> urls)
        {
            throw new InvalidOperationException();
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        public Task OnReloadAsync()
        {
            throw new InvalidOperationException();
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        public ICollection<Control> OnSearch(string searchTerm)
        {
            throw new InvalidOperationException();
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        public ICollection<Control> OnSearchCleared(string searchTerm)
        {
            throw new InvalidOperationException();
        }

        /// <exception cref="InvalidOperationException">Ignore.</exception>
        public void OnSettings()
        {
            throw new InvalidOperationException();
        }
    }
}
