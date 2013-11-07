using System;
using System.Globalization;
using System.IO;
using System.Web.Hosting;
using Pulsus.Targets;

namespace Pulsus.Internal
{
    public static class PulsusDebugger
    {
        internal static bool Debug { get; set; }
        internal static bool DebugVerbose { get; set; }
        internal static string DebugFile { get; set; }

        public static void Error(string message, params object[] args)
        {
            Error(null, null, message, args);
        }

        public static void Error(Target target, string message, params object[] args)
        {
            Error(target, null, message, args);
        }

        public static void Error(Exception ex, string message = null, params object[] args)
        {
            Error(null, ex, message, args);
        }

        public static void Error(Target target, Exception ex, string message = null, params object[] args)
        {
            WriteInternal("[Error] " + message, args, target, ex);
        }

        public static void Write(string message, params object[] args)
        {
            Write(null, message, args);
        }

        public static void Write(Target target, string message, params object[] args)
        {
            if (!DebugVerbose)
                return;

            WriteInternal(message, args, target, null);
        }

        private static void WriteInternal(string message, object[] args, Target target, Exception ex)
        {
            if (!Debug)
                return;
            
            var dateString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff", CultureInfo.InvariantCulture);
            var targetString = target != null ? string.Format("[{0}] ", target.Name) : string.Empty;
            var messageString = args != null && args.Length > 0 ? string.Format(CultureInfo.InvariantCulture, message, args) : message;
            var exceptionString = ex != null ? Environment.NewLine + ex : string.Empty;

            try
            {
                var debugFile = DebugFile;
                if (HostingEnvironment.IsHosted)
                {
                    if (string.IsNullOrEmpty(debugFile))
                        debugFile = "~/App_Data/pulsus_log.txt";
                    
                    if (debugFile.StartsWith("~/"))
                        debugFile = HostingEnvironment.MapPath(debugFile);
                }
                else
                {
                    if (string.IsNullOrEmpty(debugFile))
                        debugFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "pulsus_log.txt");

                }

                using (var textWriter = File.AppendText(debugFile))
                {
                    textWriter.WriteLine("{0} {1}{2}{3}", dateString, targetString, messageString, exceptionString);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
