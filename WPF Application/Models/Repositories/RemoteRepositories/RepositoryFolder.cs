using KallitheaKlone.Models.Repositories.RemoteRepositories;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace KallitheaKlone.WPF.Models.Repositories.RemoteRepositories
{
    public class RepositoryFolder : IRepositoryFolder<RepositoryFolder, Repository>
    {
        //  Properties
        //  ==========

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("childFolders")]
        public ICollection<RepositoryFolder> ChildFolders { get; set; } = new List<RepositoryFolder>();

        [JsonProperty("childRepositories")]
        public ICollection<Repository> ChildRepositories { get; set; } = new List<Repository>();
    }
}
