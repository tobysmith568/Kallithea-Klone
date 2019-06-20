using KallitheaKlone.Models.JSONConverter;
using Newtonsoft.Json;

namespace KallitheaKlone.WPF.Models.JSONConverter
{
    public class NewtonSoftJSONConverter : IJSONConverter
    {
        private static readonly JsonSerializerSettings settings;

        //  Constructors
        //  ============

        static NewtonSoftJSONConverter()
        {
            settings = new JsonSerializerSettings();
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        }

        //  Methods
        //  =======

        public T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public string ToJson<T>(T model)
        {
            return JsonConvert.SerializeObject(model, settings);
        }
    }
}
