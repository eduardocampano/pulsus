using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace Pulsus.Internal
{
	internal static class PulsusLogger
	{
		public static readonly string Version = Assembly.GetAssembly(typeof(PulsusLogger)).GetName().Version.ToString();
		public static readonly string WebSite = Constants.Info.WebSite;
		
		public static void Error(Exception ex, string message = null, params object[] args)
		{
			if (message == null)
				WriteInternal("Error: " + ex);
			else
				WriteInternal("Error " + string.Format(CultureInfo.InvariantCulture, message, args) +  " " + ex);
		}

		public static void Write(string message, params object[] args)
		{
			if (!LogManager.Configuration.DebugVerbose)
				return;

			WriteInternal(string.Format(CultureInfo.InvariantCulture, message, args));
		}

		private static void WriteInternal(string message)
		{
			var configuration = LogManager.Configuration;

			if (configuration.DebugFile == null)
				return;

			try
			{
				using (var textWriter = File.AppendText(configuration.DebugFile))
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
