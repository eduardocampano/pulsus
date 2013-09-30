using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Pulsus.Configuration;

namespace Pulsus.Internal
{
	internal class EmailTemplateModel
	{
		public EmailTemplateModel(LoggingEvent loggingEvent)
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

			Footer = string.Format(CultureInfo.InvariantCulture, "Pulsus | {0} | {1}", Constants.Version, Constants.WebSite);
		}

		public string Title { get; set; }
	    public string TitleUri { get; set; }
	    public string Subject { get; set; }
		public string Footer { get; private set; }
		public LoggingEvent LoggingEvent { get; private set; }
		public string LevelClass { get; private set; }
		public string LevelText { get; private set; }
		public PulsusConfiguration Settings { get; private set; }
		public string StackTrace { get; private set; }
		public HttpContextInformation HttpContextInformation { get; private set; }
		public ExceptionInformation ExceptionInformation { get; private set; }
		public SqlInformation SqlInformation { get; private set; }
		public IDictionary<string, object> CustomData { get; private set; }
	    public string IpAddressInfoUri { get; set; }
	    public string UserAgentInfoUri { get; set; }

	    public string GetFormattedTitle()
	    {
	        var title = Title;
	        if (title.IsNullOrEmpty())
	            title = LoggingEvent.Text;
	        else
	            title = Title.Format(LoggingEvent);

	        if (TitleUri.IsNullOrEmpty())
	            return title;

	        var titleUri = string.Format(CultureInfo.InvariantCulture, TitleUri, LoggingEvent.EventId);
	        return string.Format(CultureInfo.InvariantCulture, "<a class=\"link\" href=\"{0}\">{1}</a>", titleUri, title);
	    }

	    public string GetFormattedSubject()
        {
            var subject = Subject;

            if (string.IsNullOrEmpty(subject))
                subject = string.Format(CultureInfo.InvariantCulture, "[{0}] {1}{2}", LevelText, LoggingEvent.Text, LoggingEvent.Value.HasValue ? " VALUE: " + LoggingEvent.Value : string.Empty);
            else
                subject = subject.Format(LoggingEvent);

            // remove invalid line breaks
            subject = subject.Replace('\r', ' ').Replace('\n', ' ');

            if (subject.Length > 168)
                subject = subject.Substring(0, 168);

            return subject;
        }

	    public string GetFormattedIpAddress(string ipAddress)
	    {
	        if (ipAddress.IsNullOrEmpty() || IpAddressInfoUri.IsNullOrEmpty())
	            return ipAddress;

	        var ipAddressInfoUri = string.Format(IpAddressInfoUri, HttpUtility.UrlEncode(ipAddress));
            return string.Format(CultureInfo.InvariantCulture, "<a href=\"{0}\">{1}</a>", ipAddressInfoUri, ipAddress);
	    }

	    public string GetFormattedUserAgent(string userAgent)
	    {
	        if (userAgent == null || UserAgentInfoUri.IsNullOrEmpty())
	            return userAgent;

	        var userAgentInfoUri = string.Format(UserAgentInfoUri, HttpUtility.UrlEncode(userAgent));
            return string.Format(CultureInfo.InvariantCulture, "<a href=\"{0}\">{1}</a>", userAgentInfoUri, userAgent);
	    }
	}
}
