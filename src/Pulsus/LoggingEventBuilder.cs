using System;
using System.Linq;
using Pulsus.Internal;

namespace Pulsus
{
    public enum PushOptions
    {
        Default = 0,
        Async = 1,
        Sync = 2
    }

	public class LoggingEventBuilder : LoggingEventBuilderBase<LoggingEventBuilder>
	{
        public LoggingEventBuilder()
            : this(null)
        {
        }

        public LoggingEventBuilder(LoggingEvent loggingEvent)
            : base(loggingEvent)
	    {
	    }
	}

	public abstract class LoggingEventBuilderBase<T> : ILoggingEventBuilder<T> where T : class, ILoggingEventBuilder<T>, new()
	{
		protected LoggingEvent _loggingEvent;
		
		protected LoggingEventBuilderBase(LoggingEvent loggingEvent = null)
        {
            _loggingEvent = loggingEvent ?? new LoggingEvent();
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
			if (args == null || !args.Any())
				_loggingEvent.Text = text;
			else
				_loggingEvent.Text = string.Format(text, args);
			return this as T;
		}

		public virtual T AddData(string key, object value)
		{
			_loggingEvent.Data[key] = value;
			return this as T;
		}

		public virtual T Date(DateTime date, bool useUtc)
		{
			_loggingEvent.Date = useUtc ? date.ToUniversalTime() : date;
			return this as T;
		}

		public virtual T AddTags(params string[] tags)
		{
            foreach (var tag in TagHelpers.Clean(tags))
			    _loggingEvent.Tags.Add(tag);
			return this as T;
		}

		public virtual T User(string user, params object[] args)
		{
			if (args == null || !args.Any())
				_loggingEvent.User = user;
			else
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

		public virtual T Push(PushOptions pushOptions = PushOptions.Default)
		{
			AddConfiguration();

            LogManager.Push(_loggingEvent);

			return this as T;
		}

		protected void AddConfiguration()
		{
			var configurationTags = LogManager.Configuration.Tags;
            if (!string.IsNullOrEmpty(configurationTags))
            {
                foreach (var tag in TagHelpers.Clean(configurationTags))
                    _loggingEvent.Tags.Add(tag);
            }
		}
	}
}
