using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Web.Hosting;

namespace Pulsus.Internal
{
    internal static class PulsusDebugger
    {
        public static readonly string Version = Assembly.GetAssembly(typeof(PulsusDebugger)).GetName().Version.ToString();
        public static readonly string WebSite = Constants.Info.WebSite;

        public static bool Debug { get; set; }
        public static bool DebugVerbose { get; set; }
        public static string DebugFile { get; set; }

        public static void Error(Exception ex, string message = null, params object[] args)
        {
            if (message == null)
                WriteInternal("Error: " + ex);
            else
                WriteInternal("Error " + string.Format(CultureInfo.InvariantCulture, message, args) +  " " + ex);
        }

        public static void Write(string message, params object[] args)
        {
            if (!DebugVerbose)
                return;

            WriteInternal(string.Format(CultureInfo.InvariantCulture, message, args));
        }

        private static void WriteInternal(string message)
        {
            if (!Debug)
                return;

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
                    textWriter.WriteLine("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff", CultureInfo.InvariantCulture), message);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
