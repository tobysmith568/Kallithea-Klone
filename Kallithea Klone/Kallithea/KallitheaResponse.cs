using Newtonsoft.Json;

namespace Kallithea_Klone.Kallithea
{
    public class KallitheaResponse<T>
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
