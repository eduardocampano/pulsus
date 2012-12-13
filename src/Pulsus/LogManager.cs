using System;
using System.Collections.Generic;
using Pulsus.Configuration;
using Pulsus.Internal;
using Pulsus.Repositories;
using Pulsus.Targets;

namespace Pulsus
{
	public static class LogManager
	{
        private static IPulsusSettings _settings;
        private static IEventFactory _eventsFactory;
		private static IEventDispatcher _eventDispatcher;

        static LogManager()
        {
            _settings = ConfigurationManager.GetSettings();
            _eventsFactory = new DefaultEventFactory();
			_eventDispatcher = new DefaultEventDispatcher(new List<ITarget>()
			{
				new SyncWrapperTarget(new MsSqlTarget(_settings, new MsSqlLoggingEventRepository(_settings.MsSql))),
				new AsyncWrapperTarget(new ServerTarget(_settings)),
				new AsyncWrapperTarget(new EmailTarget(_settings))
			});
        }

		public static IEventFactory EventFactory
		{
			get
			{
				return _eventsFactory;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				_eventsFactory = value;	
			}
		}

		public static IEventDispatcher EventDispatcher
		{
			get
			{
				return _eventDispatcher;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				_eventDispatcher = value;
			}
		}

		public static void Push(LoggingEvent loggingEvent)
		{
			Push(new[] { loggingEvent });
		}

		public static void Push(LoggingEvent[] loggingEvents)
		{
			try
			{
				_eventDispatcher.Push(loggingEvents);
			}
			catch (Exception ex)
			{
				PulsusLogger.Error(ex);
				// swallow all exceptions
			}
		}

        public static IPulsusSettings Settings
        {
            get 
            { 
                return _settings; 
            }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				_settings = value;
			}
        }
	}
}
