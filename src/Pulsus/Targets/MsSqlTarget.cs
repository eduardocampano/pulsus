using System;
using Pulsus.Configuration;
using Pulsus.Repositories;

namespace Pulsus
{
	public class MsSqlTarget : ITarget
	{
		private readonly ILoggingEventRepository _repository;
		private readonly IMsSqlSettings _settings;

		public MsSqlTarget() : this(ConfigurationManager.Settings.MsSql)
		{
		}

		public MsSqlTarget(IMsSqlSettings settings) : this(settings, new MsSqlLoggingEventRepository())
		{ 
		}

		public MsSqlTarget(IMsSqlSettings settings, ILoggingEventRepository repository)
		{
			_settings = settings;
			_repository = repository;
			Initialize();
		}

		public void Log(LoggingEvent loggingEvent)
		{
			if (loggingEvent == null)
				throw new ArgumentNullException("loggingEvent");

			if (!_settings.Enabled)
				return;

			_repository.Save(loggingEvent);
		}

		protected void Initialize()
		{
			if (!_settings.Enabled)
				return;

			_repository.Initialize();
		}
	}
}
