using System.Collections.Generic;

namespace Pulsus
{
	public interface IEventDispatcher
	{
		void Push(LoggingEvent loggingEvent, bool async = true);
        IEnumerable<ITarget> DefaultTargets { get; set; } 
	}
}
