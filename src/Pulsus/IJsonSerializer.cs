using System;

namespace Pulsus
{
    public interface IJsonSerializer
    {
        string SerializeObject(object value);
        object DeserializeObject(string json, Type type);
        T DeserializeObject<T>(string json);
    }
}
