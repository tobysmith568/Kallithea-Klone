using Newtonsoft.Json;

namespace KallitheaKlone.Models.Kallithea
{
    public class Repository
    {
        //  Properties
        //  ==========

        [JsonProperty("repo_type")]
        public string Type { get; set; }

        [JsonProperty("repo_name")]
        public string URL { get; set; }
    }
}
