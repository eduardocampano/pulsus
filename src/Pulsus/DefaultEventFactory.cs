using System;
using System.Collections.Generic;
using System.Web.Hosting;

namespace Pulsus
{
	public class DefaultEventFactory : IEventFactory
	{
		public virtual LoggingEventBuilder Create()
		{
			return Create<LoggingEventBuilder>();
		}
		
		public virtual T Create<T>() where T : class, ILoggingEventBuilder<T>, new()
		{
			var loggingEventBuilder = new T()
										.Timestamp(DateTime.Now, true)
										.Level(LogManager.DefaultEventLevel)
										.Source(HostingEnvironment.IsHosted ? HostingEnvironment.SiteName : Environment.MachineName);

			if (LogManager.IncludeHttpContext)
				loggingEventBuilder.AddHttpContext();

			if (LogManager.IncludeStackTrace)
				loggingEventBuilder.AddStrackTrace();

			return loggingEventBuilder;
		}
	}
}
