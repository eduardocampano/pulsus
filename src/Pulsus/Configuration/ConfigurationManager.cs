using System.Web.Configuration;

namespace Pulsus.Configuration
{
	internal static class ConfigurationManager
	{
		private static IPulsusSettings _configuration;

		public static IPulsusSettings Settings
		{
			get
			{
				if (_configuration == null)
				{
					var pulsusSection = (PulsusSection)System.Configuration.ConfigurationManager.GetSection("pulsus");
					_configuration = new PulsusSectionWrapper(pulsusSection);
				}

				return _configuration;
			}
		}
	}
}
