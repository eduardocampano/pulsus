using System.ComponentModel;
using Microsoft.SharePoint.Administration;
using Pulsus.Internal;
using Pulsus.SharePoint.Core;
using Pulsus.Targets;

namespace Pulsus.SharePoint.Targets
{
	public class ULSTarget : Target
	{
		private const string DefaultFormat = "{EventId} [{Level}] {Text}";
		
		[DefaultValue(true)]
		public bool WriteTrace { get; set; }

		[DefaultValue(false)]
		public bool WriteEvent { get; set; }

		[DefaultValue(DefaultFormat)]
		public string Format { get; set; }

		public override void Push(LoggingEvent[] loggingEvents)
		{
			foreach (var loggingEvent in loggingEvents)
			{
				loggingEvent.Text = loggingEvent.Text.Substring(0, loggingEvent.Text.Length - 1);
				var messageString = (string.IsNullOrEmpty(Format) ? DefaultFormat : Format).Format(loggingEvent);

				if (WriteTrace)
				{
					var traceSeverity = GetTraceSeverity(loggingEvent);
					ULSLoggingService.WriteTrace(traceSeverity, messageString);
					PulsusDebugger.Write(this, "WriteTrace for event {0}", loggingEvent.EventId);
				}

				if (WriteEvent)
				{
					var eventSeverity = GetEventSeverity(loggingEvent);
					ULSLoggingService.WriteEvent(eventSeverity, messageString);
					PulsusDebugger.Write(this, "WriteEvent for event {0}", loggingEvent.EventId);
				}
			}
		}

		private TraceSeverity GetTraceSeverity(LoggingEvent loggingEvent)
		{
			var loggingEventLevel = loggingEvent.Level;

			if (loggingEventLevel == LoggingEventLevel.None)
				return TraceSeverity.None;
			if (loggingEventLevel == LoggingEventLevel.Trace)
				return TraceSeverity.Verbose;
			if (loggingEventLevel == LoggingEventLevel.Debug)
				return TraceSeverity.Verbose;
			if (loggingEventLevel == LoggingEventLevel.Information)
				return TraceSeverity.Verbose;
			if (loggingEventLevel == LoggingEventLevel.Warning)
				return TraceSeverity.Monitorable;
			if (loggingEventLevel == LoggingEventLevel.Error)
				return TraceSeverity.Medium;
			if (loggingEventLevel == LoggingEventLevel.Alert)
				return TraceSeverity.High;

			return TraceSeverity.None;
		}

		private EventSeverity GetEventSeverity(LoggingEvent loggingEvent)
		{
			var loggingEventLevel = loggingEvent.Level;

			if (loggingEventLevel == LoggingEventLevel.None)
				return EventSeverity.None;
			if (loggingEventLevel == LoggingEventLevel.Trace)
				return EventSeverity.Verbose;
			if (loggingEventLevel == LoggingEventLevel.Debug)
				return EventSeverity.Verbose;
			if (loggingEventLevel == LoggingEventLevel.Information)
				return EventSeverity.Information;
			if (loggingEventLevel == LoggingEventLevel.Warning)
				return EventSeverity.Warning;
			if (loggingEventLevel == LoggingEventLevel.Error)
				return EventSeverity.Error;
			if (loggingEventLevel == LoggingEventLevel.Alert)
				return EventSeverity.ErrorCritical;

			return EventSeverity.None;
		}
	}
}
