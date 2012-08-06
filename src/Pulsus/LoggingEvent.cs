using System;
using System.Collections.Generic;

namespace Pulsus
{
	public class LoggingEvent
	{
		public LoggingEvent()
		{
			this.EventId = Guid.NewGuid();
			this.Tags = new List<string>();
			this.Data = new Dictionary<string, object>();
		}

		public Guid EventId { get; set; }
		public DateTime Timestamp { get; set; }
		public int Level { get; set; }
		public double? Value { get; set; }
		public string Text { get; set; }
		public List<string> Tags { get; set; }
		public Dictionary<string, object> Data { get; set; }
		public string User { get; set; }
		public string Source { get; set; }
	}
}
