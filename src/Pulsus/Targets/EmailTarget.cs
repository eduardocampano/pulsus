using Pulsus.Configuration;

namespace Pulsus
{
	public class EmailTarget : ITarget
	{
		private readonly IEmailSettings _settings;
		
		public EmailTarget() : this(ConfigurationManager.Settings.Email)
		{
		}

		public EmailTarget(IEmailSettings settings)
		{
			_settings = settings;
		}

		public void Log(LoggingEvent loggingEvent)
		{
		}
	}
}
