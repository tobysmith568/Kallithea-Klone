using Newtonsoft.Json;

namespace KallitheaKlone.Models.Repositories.Kallithea
{
    public class RepositoryResponse : IKallitheaResponse
    {
        //  JSON Properties
        //  ===============

        [JsonProperty("repo_name")]
        public string URL { get; set; }
    }
}
