using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Pulsus.Internal;

namespace Pulsus.Server
{
	public class LogController : ApiController
	{
		public HttpResponseMessage Get([FromUri] LoggingEvent loggingEvent)
		{
			loggingEvent.MachineName = EnvironmentHelpers.TryGetMachineName();
			loggingEvent.Count = 1;

			if (loggingEvent.Level <= 0)
				loggingEvent.Level = LoggingEventLevel.Information;

			if (loggingEvent.Date == DateTime.MinValue)
				loggingEvent.Date = DateTime.UtcNow;

			new LoggingEventBuilder(loggingEvent)
						.AddHttpContext()
						.Push();

			return Request.CreateResponse(HttpStatusCode.OK, new { loggingEvent.EventId });
		}

		public HttpResponseMessage Post(LoggingEvent[] loggingEvents)
		{
			if (loggingEvents == null || !loggingEvents.Any())
				ModelState.AddModelError("Events", "No events to process");

			if (!ModelState.IsValid)
				return Request.CreateResponse(HttpStatusCode.BadRequest);

			foreach (var loggingEvent in loggingEvents)
			{
				Validate(loggingEvent);
			}
			
			LogManager.Push(loggingEvents);

			return Request.CreateResponse(HttpStatusCode.OK);
		}

		protected void Validate(LoggingEvent loggingEvent)
		{
			// ensure EventId
			if (string.IsNullOrEmpty(loggingEvent.EventId))
				loggingEvent.EventId = Guid.NewGuid().ToString();

			if (loggingEvent.Date == DateTime.MinValue)
				loggingEvent.Date = DateTime.UtcNow;
		}
	}
}
