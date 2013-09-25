using System;
using Newtonsoft.Json;

namespace Pulsus
{
    public class JsonNetSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public JsonNetSerializer() : this(null)
        {
        }

        public JsonNetSerializer(JsonSerializerSettings settings)
        {
            _settings = settings ?? new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None
            };
        }

        public string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, _settings);
        }

        public object DeserializeObject(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, _settings);
        }

        public T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _settings);
        }
    }
}
