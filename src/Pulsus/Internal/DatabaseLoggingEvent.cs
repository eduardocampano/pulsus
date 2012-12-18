using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Pulsus.Internal
{
	internal class DatabaseLoggingEvent
	{
		public static DatabaseLoggingEvent Serialize(LoggingEvent loggingEvent)
		{
			var result = new DatabaseLoggingEvent();
			result.EventId = loggingEvent.EventId;
			result.LogKey = loggingEvent.LogKey;
			result.Date = loggingEvent.Date;
			result.Level = loggingEvent.Level;
			result.Value = loggingEvent.Value;
			result.Text = loggingEvent.Text;
			result.Tags = string.Join(" ", loggingEvent.Tags.ToArray());
			result.Data = JsonConvert.SerializeObject(loggingEvent.Data);

			result.MachineName = loggingEvent.MachineName;
			result.User = loggingEvent.User;

			var httpContextInfo = loggingEvent.GetData<HttpContextInformation>(Constants.DataKeys.HttpContext);
			if (httpContextInfo != null)
			{
				result.Host = httpContextInfo.Host;
				result.Url = httpContextInfo.Url;
				result.HttpMethod = httpContextInfo.Method;
				result.IpAddress = httpContextInfo.IpAddress;
			}

			var exceptionInfo = loggingEvent.GetData<ExceptionInformation>(Constants.DataKeys.Exception);
			if (exceptionInfo != null)
			{
				result.StatusCode = exceptionInfo.StatusCode;
				result.Source = exceptionInfo.Source;
			}

			result.Count = loggingEvent.Count;
			result.Hash = loggingEvent.Hash;

			return result;
		}

		public static LoggingEvent Deserialize(DatabaseLoggingEvent mssqlLoggingEvent)
		{
			var result = new LoggingEvent();
			result.EventId = mssqlLoggingEvent.EventId;
			result.LogKey = mssqlLoggingEvent.LogKey;
			result.Date = mssqlLoggingEvent.Date;
			result.Level = mssqlLoggingEvent.Level;
			result.Value = mssqlLoggingEvent.Value;
			result.Text = mssqlLoggingEvent.Text;
			result.Tags = TagHelpers.Clean(mssqlLoggingEvent.Tags).ToList();
			result.Data = JsonConvert.DeserializeObject<Dictionary<string, object>>(mssqlLoggingEvent.Data);

			result.MachineName = mssqlLoggingEvent.MachineName;
			result.User = mssqlLoggingEvent.User;

			result.Count = mssqlLoggingEvent.Count;
			result.Hash = mssqlLoggingEvent.Hash;

			return result;
		}

		public Guid EventId { get; set; }
		public DateTime Date { get; set; }
		public string LogKey { get; set; }
		public string MachineName { get; set; }
		public int Level { get; set; }
		public double? Value { get; set; }
		public string Text { get; set; }
		public string Tags { get; set; }
		public string Data { get; set; }
		public string User { get; set; }

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
