using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Pulsus.Internal
{
    internal class EmailTemplateModel
    {
        public EmailTemplateModel(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
                throw new ArgumentNullException("loggingEvent");

            LoggingEvent = loggingEvent;
            HttpContextInformation = loggingEvent.GetData<HttpContextInformation>(Constants.DataKeys.HttpContext);
            StackTrace = loggingEvent.GetData<string>(Constants.DataKeys.StackTrace);
            SqlInformation = loggingEvent.GetData<SqlInformation>(Constants.DataKeys.SQL);
            ExceptionInformation = loggingEvent.GetData<ExceptionInformation>(Constants.DataKeys.Exception);

            GeneralSection = new Dictionary<string, object>();
            LoadGeneralSection();

            CustomData = loggingEvent.Data.Where(x => !x.Key.StartsWith("MS_", StringComparison.InvariantCultureIgnoreCase)).ToDictionary(x => x.Key, x => x.Value);
            if (!CustomData.Any())
                CustomData = null;

            RequestSection = new Dictionary<string, object>();
            LoadRequestSection(); 

            LevelText = Enum.GetName(typeof (LoggingEventLevel), LoggingEvent.Level);
            LevelClass = "green";
            var loggingEventLevelValue = (int) loggingEvent.Level;
            if (loggingEventLevelValue >= 40000 && loggingEventLevelValue <= 60000)
                LevelClass = "yellow";
            else if (loggingEventLevelValue > 60000)
                LevelClass = "red";

            Footer = string.Format(CultureInfo.InvariantCulture, "Pulsus | {0} | {1}", Constants.Version, Constants.WebSite);
        }

        public LoggingEvent LoggingEvent { get; private set; }

        public string Title { get; set; }
        public string TitleUri { get; set; }
        public string Subject { get; set; }
        public string Footer { get; private set; }
        
        public string LevelText { get; private set; }
        public string LevelClass { get; private set; }

        public string StackTrace { get; private set; }
        public HttpContextInformation HttpContextInformation { get; private set; }
        public ExceptionInformation ExceptionInformation { get; private set; }
        public SqlInformation SqlInformation { get; private set; }

        public IDictionary<string, object> GeneralSection { get; private set; }
        public IDictionary<string, object> CustomData { get; private set; }
        public IDictionary<string, object> RequestSection { get; private set; }

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

        protected void LoadGeneralSection()
        {
            GeneralSection.Clear();
            GeneralSection.Add("Date", LoggingEvent.Date + " UTC");
            GeneralSection.Add("ID", LoggingEvent.EventId);
            GeneralSection.Add("LogKey", LoggingEvent.LogKey);
            if (!LoggingEvent.ApiKey.IsNullOrEmpty())
                GeneralSection.Add("ApiKey", LoggingEvent.ApiKey);
            GeneralSection.Add("Level", Enum.GetName(typeof(LoggingEventLevel), LoggingEvent.Level));
            if (LoggingEvent.Tags.Any())
                GeneralSection.Add("Tags", string.Join(" ", LoggingEvent.Tags.ToArray()));
            GeneralSection.Add("MachineName", LoggingEvent.MachineName);

            if (ExceptionInformation != null && !ExceptionInformation.Source.IsNullOrEmpty())
                GeneralSection.Add("Source", ExceptionInformation.Source);

            GeneralSection.Add("User", LoggingEvent.User ?? "(none)");
            if (!LoggingEvent.CorrelationId.IsNullOrEmpty())
                GeneralSection.Add("CorrelationId", LoggingEvent.CorrelationId);
            if (!LoggingEvent.Psid.IsNullOrEmpty())
                GeneralSection.Add("PSID", LoggingEvent.Psid);
            if (!LoggingEvent.Ppid.IsNullOrEmpty())
                GeneralSection.Add("PPID", LoggingEvent.Ppid);
        }

        protected void LoadRequestSection()
        {
            if (HttpContextInformation == null)
                return;

            RequestSection.Add("URL", HttpContextInformation.Url);
            RequestSection.Add("Method", HttpContextInformation.Method);
            RequestSection.Add("Host", HttpContextInformation.Host);
            RequestSection.Add("Referer", HttpContextInformation.Referer);
            RequestSection.Add("IP Address", GetFormattedIpAddress(HttpContextInformation.IpAddress));
            RequestSection.Add("User Agent", GetFormattedUserAgent(HttpContextInformation.UserAgent));
        }
    }
}
