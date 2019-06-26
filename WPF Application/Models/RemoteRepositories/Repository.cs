using KallitheaKlone.Models.RemoteRepositories;
using Newtonsoft.Json;

namespace KallitheaKlone.WPF.Models.RemoteRepositories
{
    public class Repository : IRepository
    {
        //  Properties
        //  ==========

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string URL { get; set; }

        [JsonProperty("type")]
        public RepositoryType RepositoryType { get; set; }
    }
}
