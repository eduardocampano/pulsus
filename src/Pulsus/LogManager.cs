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

		private static IEventFactory _eventsFactory = new DefaultEventFactory();

		public static IEventFactory EventFactory
		{
			get
			{
				return _eventsFactory;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				_eventsFactory = value;	
			}
		}

	    private static IEventDispatcher _eventDispatcher = new DefaultEventDispatcher();
	        

		public static IEventDispatcher EventDispatcher
		{
			get { return _eventDispatcher; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				_eventDispatcher = value;
			}
		}

		public static bool DisableDefaultTargets { get; set; }
		public static bool IncludeHttpContext { get; set; }
		public static bool IncludeStackTrace { get; set; }
		public static Dictionary<string, Predicate<Exception>> ExceptionsToIgnore { get; private set; }
		public static LoggingEventLevel DefaultEventLevel { get; set; }
	}
}
