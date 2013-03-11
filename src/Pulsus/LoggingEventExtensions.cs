using System;
using Newtonsoft.Json.Linq;

namespace Pulsus
{
	public partial class LoggingEvent
	{
		public T GetData<T>(string key)
		{
			T output;

			if (this.TryGetData(key, out output))
				return output;

			return default(T);
		}

		public bool TryGetData<T>(string key, out T output)
		{
			if (!this.Data.ContainsKey(key))
			{
				output = default(T);
				return false;
			}

			var value = this.Data[key];

			// value type matches requested type
			if (value.GetType() == typeof(T))
			{
				output = (T)value;
				return true;
			}
			
			// requested type is string
			if (typeof(T) == typeof(string))
			{
				var stringValue = value as string;
				if (stringValue != null)
				{
					output = (T)Convert.ChangeType(stringValue, typeof(T));
					return true;
				}
				
				output = (T)Convert.ChangeType(value.ToString(), typeof(T));
				return false;
			}

			// value is a non deserialized JObject
			var jObject = value as JObject;
			if (jObject == null)
			{
				output = default(T);
				return false;
			}

			output = jObject.ToObject<T>();
			return output != null;
		}
	}
}
