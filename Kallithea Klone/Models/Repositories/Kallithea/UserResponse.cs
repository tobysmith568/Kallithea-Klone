using Newtonsoft.Json;

namespace KallitheaKlone.Models.Repositories.Kallithea
{
    public class UserResponse : IKallitheaResponse
    {
        //  JSON Properties
        //  ===============

        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
