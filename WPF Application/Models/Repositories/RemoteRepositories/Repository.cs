using KallitheaKlone.Models.Repositories.RemoteRepositories;
using Newtonsoft.Json;

namespace KallitheaKlone.WPF.Models.Repositories.RemoteRepositories
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
        public KallitheaKlone.Models.Repositories.RepositoryType RepositoryType { get; set; }
    }
}
