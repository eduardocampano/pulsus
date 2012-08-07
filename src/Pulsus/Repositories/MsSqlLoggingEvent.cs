using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Pulsus.Repositories
{
	internal class MsSqlLoggingEvent
	{
		public static MsSqlLoggingEvent Serialize(LoggingEvent loggingEvent)
		{
			var result = new MsSqlLoggingEvent();
			result.EventId = loggingEvent.EventId;
			result.Timestamp = loggingEvent.Timestamp;
			result.Level = loggingEvent.Level;
			result.Value = loggingEvent.Value;
			result.Text = loggingEvent.Text;
			result.Tags = string.Join(" ", loggingEvent.Tags.ToArray());
			result.Data = JsonConvert.SerializeObject(loggingEvent.Data);
			result.User = loggingEvent.User;
			result.Source = loggingEvent.Source;
			return result;
		}

		public static LoggingEvent Deserialize(MsSqlLoggingEvent mssqlLoggingEvent)
		{
			var result = new LoggingEvent();
			result.EventId = mssqlLoggingEvent.EventId;
			result.Timestamp = mssqlLoggingEvent.Timestamp;
			result.Level = mssqlLoggingEvent.Level;
			result.Value = mssqlLoggingEvent.Value;
			result.Text = mssqlLoggingEvent.Text;
			result.Tags = Pulsus.Tags.Clean(mssqlLoggingEvent.Tags).ToList();
			result.Data = JsonConvert.DeserializeObject<Dictionary<string, object>>(mssqlLoggingEvent.Data);
			result.User = mssqlLoggingEvent.User;
			result.Source = mssqlLoggingEvent.Source;
			return result;
		}

		public Guid EventId { get; set; }
		public DateTime Timestamp { get; set; }
		public int Level { get; set; }
		public double? Value { get; set; }
		public string Text { get; set; }
		public string Tags { get; set; }
		public string Data { get; set; }
		public string User { get; set; }
		public string Source { get; set; }
	}
}
