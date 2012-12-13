using System;

namespace Pulsus.Internal
{
	internal interface ITimer : IDisposable
	{
		void Change(long dueTime, long period);
	}
}
