using Newtonsoft.Json;

namespace Kallithea_Klone.Other_Classes
{
    class User
    {
        //  JSON Properties
        //  ===============

        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
