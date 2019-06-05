using Newtonsoft.Json;

namespace KallitheaKlone.Models.JSONConverter
{
    public class JSONConverter : IJSONConverter
    {
        //  Methods
        //  =======

        public T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public string ToJson<T>(T model)
        {
            return JsonConvert.SerializeObject(model);
        }
    }
}
