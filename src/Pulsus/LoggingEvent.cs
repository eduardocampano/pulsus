using System;
using System.Collections.Generic;

namespace Pulsus
{
	public partial class LoggingEvent
	{
		public LoggingEvent()
		{
			EventId = Guid.NewGuid().ToString();
			Date = DateTime.UtcNow;
			Count = 1;
			Tags = new List<string>();
			Data = new Dictionary<string, object>();
		}

		public virtual string EventId { get; set; }
		public virtual string LogKey { get; set; }
		public virtual string ApiKey { get; set; }
		public virtual DateTime Date { get; set; }
		public virtual int Level { get; set; }
		public virtual double? Value { get; set; }
		public virtual string Text { get; set; }
		public virtual IList<string> Tags { get; set; }
		public virtual IDictionary<string, object> Data { get; set; }

		public virtual string MachineName { get; set; }
		public virtual string User { get; set; }
		public virtual string Psid { get; set; }
		public virtual string Ppid { get; set; }

		public virtual int Hash { get; set; }
		public virtual int Count { get; set; }
	}
}
