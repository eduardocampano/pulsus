using System.Collections.Generic;

namespace Pulsus.Repositories
{
	public interface ILoggingEventRepository
	{
		void Initialize();
		void Save(LoggingEvent loggingEvent);
		IEnumerable<LoggingEvent> Retrieve();
	}
}
