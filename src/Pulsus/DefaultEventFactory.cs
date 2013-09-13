using System;
using Pulsus.Configuration;
using Pulsus.Internal;

namespace Pulsus
{
    public class DefaultEventFactory : IEventFactory
    {
        private readonly PulsusConfiguration _configuration;

        public DefaultEventFactory() : this(LogManager.Configuration)
        {
        }

        public DefaultEventFactory(PulsusConfiguration configuration)
        {
            _configuration = configuration;
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
                                                .Level(_configuration.DefaultEventLevel);

            loggingEventBuilder.LoggingEvent.LogKey = _configuration.LogKey;
            loggingEventBuilder.LoggingEvent.MachineName = EnvironmentHelpers.TryGetMachineName();
            loggingEventBuilder.LoggingEvent.Count = 1;

            if (_configuration.IncludeHttpContext)
                loggingEventBuilder.AddHttpContext();

            if (_configuration.IncludeStackTrace)
                loggingEventBuilder.AddStrackTrace();
        }
    }
}
