namespace Pulsus.Configuration
{
	internal static class ConfigurationManager
	{
		public static IPulsusSettings GetSettings()
		{
            var pulsusSection = (PulsusSection)System.Configuration.ConfigurationManager.GetSection("pulsus");
            return new PulsusSettings(pulsusSection ?? new PulsusSection());
		}
	}
}
