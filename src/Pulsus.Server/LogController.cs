using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Pulsus.Internal;

namespace Pulsus.Server
{
	public class LogController : ApiController
	{
		private const string LogKeyHeader = "X-PULSUS-LOGKEY";
		private const string ApiKeyHeader = "X-PULSUS-APIKEY";

		public HttpResponseMessage Get([FromUri] LoggingEvent loggingEvent, [FromUri] string apiKey)
		{
			loggingEvent.MachineName = EnvironmentHelpers.TryGetMachineName();
			loggingEvent.Count = 1;

			if (loggingEvent.Level <= 0)
				loggingEvent.Level = (int)LoggingEventLevel.Information;

			if (loggingEvent.Date == DateTime.MinValue)
				loggingEvent.Date = DateTime.UtcNow;

			new LoggingEventBuilder(loggingEvent)
						.AddHttpContext()
						.Push();

			return Request.CreateResponse(HttpStatusCode.OK, new { loggingEvent.EventId });
		}

		public HttpResponseMessage Post(LoggingEvent[] loggingEvents)
		{
			var logKey = GetHeader(LogKeyHeader);
			var apiKey = GetHeader(ApiKeyHeader);

			if (loggingEvents == null || !loggingEvents.Any())
				ModelState.AddModelError("Events", "No events to process");

			if (!ModelState.IsValid)
				return Request.CreateResponse(HttpStatusCode.BadRequest);

			foreach (var loggingEvent in loggingEvents)
			{
				Validate(logKey, loggingEvent);
			}
			
			LogManager.Push(loggingEvents);

			return Request.CreateResponse(HttpStatusCode.OK);
		}

		protected void Validate(string logKey, LoggingEvent loggingEvent)
		{
			// override log key
			loggingEvent.LogKey = logKey;

			// ensure EventId
			if (loggingEvent.EventId == Guid.Empty)
				loggingEvent.EventId = Guid.NewGuid();

			if (loggingEvent.Date == DateTime.MinValue)
				loggingEvent.Date = DateTime.UtcNow;
		}

		protected string GetHeader(string name, bool mandatory = true)
		{
			IEnumerable<string> values;
			if (ControllerContext.Request.Headers.TryGetValues(name, out values))
				return values.FirstOrDefault();

			if (mandatory)
				ModelState.AddModelError(name, string.Format(CultureInfo.InvariantCulture, "The {0} header is invalid", name));

			return null;
		}
	}
}
