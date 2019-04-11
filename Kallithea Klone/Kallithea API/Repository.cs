using Newtonsoft.Json;

namespace Kallithea_Klone.Kallithea
{
    public class Repository
    {
        //  JSON Properties
        //  ===============

        [JsonProperty("repo_name")]
        public string URL { get; set; }
    }
}
