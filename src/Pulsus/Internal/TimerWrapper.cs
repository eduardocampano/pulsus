using System.Threading;

namespace Pulsus.Internal
{
	internal class TimerWrapper : ITimer
	{
		private readonly Timer _timer;

		public TimerWrapper(TimerCallback callback)
		{
			_timer = new Timer(callback, null, Timeout.Infinite, Timeout.Infinite);
		}

		public void Change(long dueTime, long period)
		{
			_timer.Change(dueTime, period);
		}

		public void Dispose()
		{
			_timer.Dispose();
		}
	}
}
