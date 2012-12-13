using System;
using Pulsus.Configuration;
using Pulsus.Internal;

namespace Pulsus
{
	public class DefaultEventFactory : IEventFactory
	{
        private readonly IPulsusSettings _settings;

        public DefaultEventFactory() : this(LogManager.Settings)
	    {
	    }

        public DefaultEventFactory(IPulsusSettings settings)
        {
            _settings = settings;
        }

        public virtual LoggingEventBuilder Create()
		{
            return Create<LoggingEventBuilder>();
		}
		
		public virtual T Create<T>() where T : class, ILoggingEventBuilder<T>, new()
		{
            var instance = new T();
            Initialize(instance);
            return instance;
		}

        protected virtual void Initialize<T>(T instance) where T : class, ILoggingEventBuilder<T>
	    {
	        var loggingEventBuilder = instance.Date(DateTime.Now, true)
												.Level(_settings.DefaultEventLevel);

			loggingEventBuilder.LoggingEvent.LogKey = _settings.LogKey;
			loggingEventBuilder.LoggingEvent.MachineName = EnvironmentHelpers.TryGetMachineName();
			loggingEventBuilder.LoggingEvent.Count = 1;

			if (_settings.IncludeHttpContext)
				loggingEventBuilder.AddHttpContext();

            if (_settings.IncludeStackTrace)
				loggingEventBuilder.AddStrackTrace();
	    }
	}
}
