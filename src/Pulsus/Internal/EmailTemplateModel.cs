using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Pulsus.Configuration;

namespace Pulsus.Internal
{
	internal class EmailTemplateModel
	{
		public EmailTemplateModel(LoggingEvent loggingEvent, string subject, string link)
		{
			if (loggingEvent == null)
				throw new ArgumentNullException("loggingEvent");

			LoggingEvent = loggingEvent;
			StackTrace = loggingEvent.GetData<string>(Constants.DataKeys.StackTrace);
			SqlInformation = loggingEvent.GetData<SqlInformation>(Constants.DataKeys.SQL);
			ExceptionInformation = loggingEvent.GetData<ExceptionInformation>(Constants.DataKeys.Exception);
			HttpContextInformation = loggingEvent.GetData<HttpContextInformation>(Constants.DataKeys.HttpContext);
			CustomData = loggingEvent.Data.Where(x => !x.Key.StartsWith("MS_", StringComparison.InvariantCultureIgnoreCase)).ToDictionary(x => x.Key, x => x.Value);
			if (!CustomData.Any())
				CustomData = null;

			LevelText = Enum.GetName(typeof(LoggingEventLevel), loggingEvent.Level);
			LevelClass = "green";
			if (loggingEvent.Level >= 40000 && loggingEvent.Level <= 60000)
				LevelClass = "yellow";
			else if (loggingEvent.Level > 60000)
				LevelClass = "red";

			Link = link.Format(loggingEvent);
			Subject = string.IsNullOrEmpty(subject) ? string.Format(CultureInfo.InvariantCulture, "[{0}] {1}{2}", LevelText, loggingEvent.Text, loggingEvent.Value.HasValue ? " VALUE: " + loggingEvent.Value : string.Empty) : subject.Format(loggingEvent);

			Title = string.Format(CultureInfo.InvariantCulture, "{0}", loggingEvent.Text);
			Footer = string.Format(CultureInfo.InvariantCulture, "Pulsus | {0} | {1}", PulsusLogger.Version, PulsusLogger.WebSite);
		}

		public string Title { get; private set; }
		public string Subject { get; private set; }
		public string Footer { get; private set; }
		public LoggingEvent LoggingEvent { get; private set; }
		public string LevelClass { get; private set; }
		public string LevelText { get; private set; }
		public PulsusConfiguration Settings { get; private set; }
		public string Link { get; private set; }
		public string StackTrace { get; private set; }
		public HttpContextInformation HttpContextInformation { get; private set; }
		public ExceptionInformation ExceptionInformation { get; private set; }
		public SqlInformation SqlInformation { get; private set; }
		public IDictionary<string, object> CustomData { get; private set; }
	}
}
