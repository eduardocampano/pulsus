using System;
using System.Collections.Generic;
using System.Web;

namespace Pulsus
{
	public static class LogManager
	{
		static LogManager()
		{
			DefaultEventLevel = LoggingEventLevel.Information;
			ExceptionsToIgnore = new Dictionary<string, Predicate<Exception>>();
			ExceptionsToIgnore.Add("HttpException404", e => e is HttpException && ((HttpException)e).GetHttpCode() == 404);
			ExceptionsToIgnore.Add("PotentiallyDangerousRequests", e => e is HttpException && e.Message.StartsWith("A potentially dangerous Request", StringComparison.InvariantCulture));
		}

		private static IEventFactory eventsFactory = new DefaultEventFactory();

		public static IEventFactory EventFactory
		{
			get
			{
				return eventsFactory;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				eventsFactory = value;	
			}
		}

		private static IEventDispatcher eventDispatcher = new DefaultEventDispatcher();

		public static IEventDispatcher EventDispatcher
		{
			get
			{
				return eventDispatcher;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				eventDispatcher = value;
			}
		}

		public static bool DisableDefaultTargets { get; set; }
		public static bool IncludeHttpContext { get; set; }
		public static bool IncludeStackTrace { get; set; }
		public static Dictionary<string, Predicate<Exception>> ExceptionsToIgnore { get; private set; }
		public static LoggingEventLevel DefaultEventLevel { get; set; }
	}
}
