using System;
using System.Collections.Generic;

namespace Pulsus.Configuration
{
    public interface IPulsusSettings
    {
		string LogKey { get; set; }
        string Tags { get; set; }
        bool IncludeHttpContext { get; set; }
        bool IncludeStackTrace { get; set; }
		bool Debug { get; set; }
		string DebugFile { get; set; }

        IDictionary<string, Predicate<Exception>> ExceptionsToIgnore { get; set; }
        LoggingEventLevel DefaultEventLevel { get; set; }
        IServerSettings Server { get; }
        IMsSqlSettings MsSql { get; }
        IEmailSettings Email { get; }
    }
}
