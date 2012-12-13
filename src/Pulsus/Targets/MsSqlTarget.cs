using System;
using Pulsus.Configuration;
using Pulsus.Repositories;

namespace Pulsus.Targets
{
	public class MsSqlTarget : ITarget
	{
		private readonly ILoggingEventRepository _repository;
		private readonly IPulsusSettings _settings;
        private bool _initialized;

		public MsSqlTarget(IPulsusSettings settings, ILoggingEventRepository repository)
		{
			_settings = settings;
			_repository = repository;
		}

		public bool Enabled
		{
			get
			{
				return _settings.MsSql.Enabled;
			}
		}

		public void Push(LoggingEvent[] loggingEvents)
		{
			if (loggingEvents == null)
				throw new ArgumentNullException("loggingEvents");

			if (!Enabled)
				return;

            if (!_initialized)
            {
                _repository.Initialize();
                _initialized = true;
            }

			_repository.Save(loggingEvents);
		}

		public override string ToString()
		{
			return string.Format("[MsSql] ConnectionName: {0}, DatabaseName: {1}", _settings.MsSql.ConnectionName, _settings.MsSql.DatabaseName);
		}
	}
}
