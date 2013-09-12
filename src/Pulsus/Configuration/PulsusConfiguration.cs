using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Xml.Linq;
using Pulsus.Internal;
using Pulsus.Targets;

namespace Pulsus.Configuration
{
    public class PulsusConfiguration
    {
        public PulsusConfiguration()
        {
            DefaultEventLevel = LoggingEventLevel.Information;
            Enabled = true;
            Debug = false;
            DebugVerbose = false;
            Targets = new Dictionary<string, Target>(StringComparer.OrdinalIgnoreCase);
            ExceptionsToIgnore = new Dictionary<string, Predicate<Exception>>(StringComparer.OrdinalIgnoreCase);

            LogKey = AppDomain.CurrentDomain.FriendlyName;
            if (HostingEnvironment.IsHosted)
                LogKey = HostingEnvironment.SiteName;
        }

        public static PulsusConfiguration Default
        {
            get
            {
                return GetConfiguration(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pulsus.config"));
            }
        }

        public static PulsusConfiguration Load(string fileName)
        {
            return GetConfiguration(fileName);
        }

        /// <summary>
        /// Gets or sets whether Pulsus is enabled. If set to false no events will be pushed to any target. The default value is true.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets whether Pulsus is in debug mode. The default valus is false. This is useful for finding errors in pulsus or targets configuration.
        /// </summary>
        public bool Debug { get; set; }
        
        /// <summary>
        /// Gets or sets the path 
        /// </summary>
        public string DebugFile { get; set; }

        /// <summary>
        /// Gets or sets whether the debug information should be sent to the Windows Event Log. The default value is false.
        /// </summary>
        public bool DebugToEventLog { get; set; }

        /// <summary>
        /// Gets or sets whether the debug file must include detailed information. If set to true detailed information will be logged to the debug file, if not, only errors will be logged. The default value is false.
        /// </summary>
        public bool DebugVerbose { get; set; }

        /// <summary>
        /// Gets or sets the default LogKey for all the events. The default value is the name of the application or IIS application.
        /// </summary>
        public string LogKey { get; set; }
        
        /// <summary>
        /// Gets or sets the tags to be added to all the events.
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets whether Pulsus must throw exceptions when an intenal error happens. The default value is false.
        /// </summary>
        public bool ThrowExceptions { get; set; }

        /// <summary>
        /// Gets or sets whether the HttpContext information must be included for all the events when available. The default value is false.
        /// </summary>
        public bool IncludeHttpContext { get; set; }

        /// <summary>
        /// Gets or sets whether the Stack Trace must be included for all the events. The default value is false.
        /// </summary>
        public bool IncludeStackTrace { get; set; }

        /// <summary>
        /// Gets or sets whether 404 status code HttpExceptions must be ignored. The default value is false.
        /// </summary>
        public bool IgnoreNotFound { get; set; }

        /// <summary>
        /// Gets or sets whether all the targets must be wrapped with an AsyncWrapperTarget. The default value is false.
        /// </summary>
        public bool Async { get; set; }

        /// <summary>
        /// Gets or sets the default event level. The default value is LoggingEventLevel.None.
        /// </summary>
        public LoggingEventLevel DefaultEventLevel { get; set; }

        /// <summary>
        /// Returns a dictionary of exception evaluators which will evaluate if the exception must be ignored or not. 
        /// </summary>
        public IDictionary<string, Predicate<Exception>> ExceptionsToIgnore { get; private set; }

        /// <summary>
        /// Returns a dictionary of the destination targets where the logging events may be pushed. A logging event may not necessary be pushed to all
        /// the targets in the dictionary because a Target can have filter and ignore conditions. 
        /// </summary>
        public IDictionary<string, Target> Targets { get; protected set; } 

        private static PulsusConfiguration GetConfiguration(string fileName)
        {
            PulsusConfiguration configuration;

            if (File.Exists(fileName))
                configuration = new PulsusXmlConfiguration(fileName);
            else
                configuration = new PulsusConfiguration();

            // default targets
            if (!configuration.Targets.Any())
            {
                var databaseTarget = new DatabaseTarget();
                TypeHelpers.LoadDefaultValues(databaseTarget);
                var wrapperTarget = new WrapperTarget(databaseTarget);
                TypeHelpers.LoadDefaultValues(wrapperTarget);
                configuration.Targets.Add("database", wrapperTarget);
            }

            if (configuration.IgnoreNotFound)
                configuration.ExceptionsToIgnore.Add("notfound", IsNotFoundException);

            SetupDebugging(configuration);

            return configuration;
        }

        private static void SetupDebugging(PulsusConfiguration configuration)
        {
            if (configuration.Debug)
            {
                if (HostingEnvironment.IsHosted)
                {
                    if (string.IsNullOrEmpty(configuration.DebugFile))
                        configuration.DebugFile = "~/App_Data/pulsus_log.txt";

                    configuration.DebugFile = HostingEnvironment.MapPath(configuration.DebugFile);
                }
                else
                {
                    if (string.IsNullOrEmpty(configuration.DebugFile))
                        configuration.DebugFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "pulsus_log.txt");
                }
            }
        }

        private static bool IsNotFoundException(Exception ex)
        {
            var httpException = ex as HttpException;
            return httpException != null && httpException.GetHttpCode() == 404;
        }
    }
}
