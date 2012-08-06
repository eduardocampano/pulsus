using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pulsus
{
	public interface IEventDispatcher
	{
		void Push(LoggingEvent loggingEvent, bool async = true);
	}
}
