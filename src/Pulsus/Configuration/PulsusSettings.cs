using System;
using System.Collections.Generic;
using System.Web;

namespace Pulsus.Configuration
{
	public class PulsusSettings : IPulsusSettings
	{
        public PulsusSettings()
        {
            ExceptionsToIgnore = new Dictionary<string, Predicate<Exception>>();
            DefaultEventLevel = LoggingEventLevel.Information;
            ExceptionsToIgnore = new Dictionary<string, Predicate<Exception>>();
            ExceptionsToIgnore.Add("HttpException404", e => e is HttpException && ((HttpException)e).GetHttpCode() == 404);
            ExceptionsToIgnore.Add("PotentiallyDangerousRequests", e => e is HttpException && e.Message.StartsWith("A potentially dangerous Request", StringComparison.InvariantCulture));
        }

		internal PulsusSettings(PulsusSection section) : this()
		{
			if (section == null)
				throw new ArgumentNullException("section");

			Debug = section.Debug;
			DebugFile = section.DebugFile;
			LogKey = section.LogKey;
            Tags = section.Tags;
            IncludeHttpContext = section.IncludeHttpContext;
            IncludeStackTrace = section.IncludeStackTrace;
            Server = new ServerSettings(section.Server);
            Email = new EmailSettings(section.Email);
            MsSql = new MsSqlSettings(section.MsSql);
		}

		public bool Debug { get; set; }
		public string DebugFile { get; set; }
		public string LogKey { get; set; }
		public string Tags { get; set; }
        public bool IncludeHttpContext { get; set; }
        public bool IncludeStackTrace { get; set; }
        public IDictionary<string, Predicate<Exception>> ExceptionsToIgnore { get; set; }
        public LoggingEventLevel DefaultEventLevel { get; set; }

		public IServerSettings Server { get; set; }
        public IMsSqlSettings MsSql { get; set; }
		public IEmailSettings Email { get; set; }
	}
}
