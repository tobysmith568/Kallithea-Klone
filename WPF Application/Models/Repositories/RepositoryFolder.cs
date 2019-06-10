using KallitheaKlone.Models.Repositories;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace KallitheaKlone.WPF.Models.Repositories
{
    public class RepositoryFolder : IRepositoryFolder<RepositoryFolder, Repository>
    {
        //  Properties
        //  ==========

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("childFolders")]
        public ICollection<RepositoryFolder> ChildFolders { get; set; }

        [JsonProperty("childRepositories")]
        public ICollection<Repository> ChildRepositories { get; set; }
    }
}
