using Kallithea_Klone.Other_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Kallithea_Klone.WPF_Controls
{
    public class Repository : CheckBox, IRepoControl
    {
        //  Properties
        //  ==========

        public SortingCategory SortingCategory { get; } = SortingCategory.Repository;
        public string LocationName { get; }
        public string RepoName { get; set; }
        public string RepoURL { get; set; }

        //  Constructors
        //  ============

        public Repository(string repoName, string repoURL, RoutedEventHandler checkedEvent) : base()
        {
            LocationName = repoName;
            RepoName = repoName;
            RepoURL = repoURL;

            Content = repoName;
            VerticalContentAlignment = VerticalAlignment.Center;
            FontSize = 18;

            Checked += checkedEvent;
            Unchecked += checkedEvent;
        }

        //  Casts
        //  =====

        public static implicit operator RepositoryData(Repository repository)
        {
            if (repository.RepoName == null || repository.RepoURL == null)
                throw new ArgumentNullException();

            return new RepositoryData
            {
                Name = repository.RepoName,
                URL = repository.RepoURL
            };
        }
    }
}
