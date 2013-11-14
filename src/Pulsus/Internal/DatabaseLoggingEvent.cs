using System;
using System.Linq;

namespace Pulsus.Internal
{
	internal class DatabaseLoggingEvent
	{
		public static DatabaseLoggingEvent Serialize(LoggingEvent loggingEvent)
		{
			var result = new DatabaseLoggingEvent();
			result.EventId = Truncate(loggingEvent.EventId, 38);
			result.LogKey = Truncate(loggingEvent.LogKey, 100);
			result.ApiKey = Truncate(loggingEvent.ApiKey, 100);
			result.Date = loggingEvent.Date;
			result.Level = (int)loggingEvent.Level;
			result.Value = loggingEvent.Value;
			result.Text = Truncate(loggingEvent.Text, 5000);
			result.Tags = Truncate(string.Join(" ", loggingEvent.Tags.ToArray()), 1000);
			result.Data = LogManager.JsonSerializer.SerializeObject(loggingEvent.Data);

			result.MachineName = Truncate(loggingEvent.MachineName, 100);
			result.User = Truncate(loggingEvent.User, 500);
			result.Psid = Truncate(loggingEvent.Psid, 50);
			result.Ppid = Truncate(loggingEvent.Ppid, 50);

			var httpContextInfo = loggingEvent.GetData<HttpContextInformation>(Constants.DataKeys.HttpContext);
			if (httpContextInfo != null)
			{
				result.Host = Truncate(httpContextInfo.Host, 255);
				result.Url = Truncate(httpContextInfo.Url, 2000);
				result.HttpMethod = Truncate(httpContextInfo.Method, 10);
				result.IpAddress = Truncate(httpContextInfo.IpAddress, 40);
			}

			var exceptionInfo = loggingEvent.GetData<ExceptionInformation>(Constants.DataKeys.Exception);
			if (exceptionInfo != null)
			{
				result.StatusCode = exceptionInfo.StatusCode;
				result.Source = Truncate(exceptionInfo.Source, 500);
			}

			result.Count = loggingEvent.Count;
			result.Hash = loggingEvent.Hash;

			return result;
		}

		private static string Truncate(string value, int length)
		{
			if (value == null)
				return null;

			if (value.Length > length)
				return value.Substring(0, length);

			return value;
		}

		public string EventId { get; set; }
		public DateTime Date { get; set; }
		public string LogKey { get; set; }
		public string ApiKey { get; set; }
		public string MachineName { get; set; }
		public int Level { get; set; }
		public double? Value { get; set; }
		public string Text { get; set; }
		public string Tags { get; set; }
		public string Data { get; set; }
		public string User { get; set; }
		public string Psid { get; set; }
		public string Ppid { get; set; }

		// denormalized info for search
		public string Host { get; set; }
		public string Url { get; set; }
		public string HttpMethod { get; set; }
		public string IpAddress { get; set; }
		public int StatusCode { get; set; }
		public string Source { get; set; }

		public int Hash { get; set; }
		public int Count { get; set; }
	}
}
