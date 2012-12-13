using System;

namespace Pulsus
{
	public interface ILoggingEventBuilder<out T> where T : class
	{
		LoggingEvent LoggingEvent { get; }
		T Date(DateTime date, bool useUtc);
		T AddData(string key, object value); 
		T AddTags(params string[] tags);
		T Text(string text, params object[] args);
		T User(string user, params object[] args);
		T Level(LoggingEventLevel level);
		T Level(int level);
	}
}
