using System.Diagnostics;
using System.Reflection;

namespace Pulsus.Internal
{
    public static class Constants
    {
        private static readonly Assembly PulsusAssembly = Assembly.GetAssembly(typeof(Constants));
        public static readonly string Version = FileVersionInfo.GetVersionInfo(PulsusAssembly.Location).FileVersion;
        public static readonly string WebSite = Info.WebSite;

        public static class DataKeys
        {
            public static readonly string StackTrace = "MS_StackTrace";
            public static readonly string Exception = "MS_Exception";
            public static readonly string SQL = "MS_SQL";
            public static readonly string HttpContext = "MS_HttpContext";
        }
        
        public static class Info
        {
            public static readonly string WebSite = "https://pulsus.codeplex.com";
        }
    }
}
