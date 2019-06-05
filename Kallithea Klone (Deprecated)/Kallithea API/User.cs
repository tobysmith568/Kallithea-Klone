using Newtonsoft.Json;

namespace Kallithea_Klone.Kallithea
{
    public class User
    {
        //  JSON Properties
        //  ===============

        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
