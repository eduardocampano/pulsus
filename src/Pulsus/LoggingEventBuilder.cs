using System;
using Newtonsoft.Json;
using Pulsus.Configuration;

namespace Pulsus
{
	public class LoggingEventBuilder : LoggingEventBuilderBase<LoggingEventBuilder>
	{
	}

	public abstract class LoggingEventBuilderBase<T> : ILoggingEventBuilder<T> where T : class, ILoggingEventBuilder<T>, new()
	{
		protected LoggingEvent _loggingEvent = new LoggingEvent();
		
		protected LoggingEventBuilderBase()
        {
        }

		internal static T Create()
		{
			return new T();
		}

		public LoggingEvent LoggingEvent
		{
			get
			{
				return _loggingEvent;
			}
		}

		public virtual T Text(string text, params object[] args)
		{
			_loggingEvent.Text = string.Format(text, args);
			return this as T;
		}

		public virtual T AddData(string key, object value)
		{
			_loggingEvent.Data[key] = value;
			return this as T;
		}

		public virtual T Source(string text, params object[] args)
		{
			_loggingEvent.Source = string.Format(text, args);
			return this as T;
		}

		public virtual T Timestamp(DateTime timestamp, bool useUtc)
		{
			_loggingEvent.Timestamp = useUtc ? timestamp.ToUniversalTime() : timestamp;
			return this as T;
		}

		public virtual T AddTags(params string[] tags)
		{
			_loggingEvent.Tags.AddRange(Tags.Clean(tags));
			return this as T;
		}

		public virtual T User(string user, params object[] args)
		{
			_loggingEvent.User = string.Format(user, args);
			return this as T;
		}

		public virtual T Level(LoggingEventLevel level)
		{
			_loggingEvent.Level = (int)level;
			return this as T;
		}

		public virtual T Level(int level)
		{
			_loggingEvent.Level = level;
			return this as T;
		}

		public virtual T Push(bool async = true)
		{
			var eventDispatcher = LogManager.EventDispatcher;
			if (eventDispatcher == null)
				throw new Exception("Could not found a dispatcher.");

			AddConfiguration();

			eventDispatcher.Push(this.LoggingEvent, async);
			return this as T;
		}

		protected void AddConfiguration()
		{
			var configurationTags = ConfigurationManager.Settings.Tags;
			if (!string.IsNullOrEmpty(configurationTags))
				_loggingEvent.Tags.AddRange(Tags.Clean(configurationTags));

			var configurationSource = ConfigurationManager.Settings.Source;
			if (!string.IsNullOrEmpty(configurationSource))
				_loggingEvent.Source = configurationSource;
		}
	}
}
