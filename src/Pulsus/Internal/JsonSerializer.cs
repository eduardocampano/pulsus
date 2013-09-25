namespace Pulsus.Internal
{
    internal static class JsonSerializer
    {
        public static string Serialize(object instance)
        {
            return SimpleJson.SerializeObject(instance);
        }

        public static T Deserialize<T>(string json)
        {
            return SimpleJson.DeserializeObject<T>(json);
        }
    }
}
