using Newtonsoft.Json;

namespace KallitheaKlone.Models.Repositories.Kallithea
{
    public class KallitheaBaseResponse<T> where T : IKallitheaResponse
    {
        //  JSON Properties
        //  ===============

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("result")]
        public T Result { get; set; }
    }
}
