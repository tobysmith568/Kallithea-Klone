using Newtonsoft.Json;

namespace KallitheaKlone.Models.Kallithea
{
    public class KallitheaBaseResponse<T>
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
