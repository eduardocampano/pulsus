using System;
using System.Collections.Generic;
using System.Linq;
using Pulsus.Internal;

namespace Pulsus.SharePoint
{
    internal class DatabaseLoggingEvent
    {
        public static LoggingEvent Deserialize(DatabaseLoggingEvent mssqlLoggingEvent)
        {
            var result = new LoggingEvent();
            result.EventId = mssqlLoggingEvent.EventId;
            result.LogKey = mssqlLoggingEvent.LogKey;
            result.ApiKey = mssqlLoggingEvent.ApiKey;
            result.Date = mssqlLoggingEvent.Date;
            result.Level = (LoggingEventLevel)Enum.Parse(typeof(LoggingEventLevel), mssqlLoggingEvent.Level.ToString());
            result.Value = mssqlLoggingEvent.Value;
            result.Text = mssqlLoggingEvent.Text;
            result.Tags = mssqlLoggingEvent.Tags.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            result.Data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            try
            {
                result.Data = LogManager.JsonSerializer.DeserializeObject<IDictionary<string, object>>(mssqlLoggingEvent.Data);
            }
            catch (Exception)
            {
            }
            
            result.MachineName = mssqlLoggingEvent.MachineName;
            result.User = mssqlLoggingEvent.User;
            result.Psid = mssqlLoggingEvent.Psid;
            result.Ppid = mssqlLoggingEvent.Ppid;

            result.Count = mssqlLoggingEvent.Count;
            result.Hash = mssqlLoggingEvent.Hash;

            return result;
        }

        public long Id { get; set; }
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