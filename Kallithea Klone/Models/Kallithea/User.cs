using Newtonsoft.Json;

namespace KallitheaKlone.Models.Kallithea
{
    public class User
    {
        //  JSON Properties
        //  ===============

        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
