using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Web.Hosting;

namespace Pulsus.Internal
{
	internal static class PulsusLogger
	{
		public static readonly string Version = Assembly.GetAssembly(typeof(PulsusLogger)).GetName().Version.ToString();
		public static readonly string WebSite = Constants.Info.WebSite;
		private readonly static string DebugFile;

		static PulsusLogger()
		{
			DebugFile = LogManager.Configuration.Debug ? LogManager.Configuration.DebugFile : null;
			if (DebugFile != null)
			{

				if (HostingEnvironment.IsHosted)
				{
					if (string.IsNullOrEmpty(DebugFile))
						DebugFile = "~/App_Data/pulsus_log.txt";

					DebugFile = HostingEnvironment.MapPath(DebugFile);
				}
				else
				{
					if (string.IsNullOrEmpty(DebugFile))
						DebugFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "pulsus_log.txt");
				}
			}
		}

		public static void Error(Exception ex, string message = null, params object[] args)
		{
			if (message == null)
				WriteInternal("Error: " + ex);
			else
				WriteInternal("Error " + string.Format(CultureInfo.InvariantCulture, message, args) +  " " + ex);
		}

		public static void Write(string message, params object[] args)
		{
			WriteInternal(string.Format(CultureInfo.InvariantCulture, message, args));
		}

		private static void WriteInternal(string message)
		{
			if (DebugFile == null)
				return;

			try
			{
				using (var textWriter = File.AppendText(DebugFile))
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
