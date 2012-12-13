using System.Collections.Generic;
using System.Linq;

namespace Pulsus.Internal
{
	internal class LoggingEventQueue
	{
		private readonly Queue<LoggingEvent> _loggingEventQueue = new Queue<LoggingEvent>();
		private readonly object _locker = new object();

		public LoggingEventQueue(int maxQueueSize = 10000)
		{
			MaxQueueSize = maxQueueSize;
		}

		public int MaxQueueSize { get; set; }

		public int QueueCount
		{
			get
			{
				return _loggingEventQueue.Count;
			}
		}

		public void Enqueue(LoggingEvent[] loggingEvents)
		{
			lock (_locker)
			{
				// ignore new logging events
				if (_loggingEventQueue.Count >= MaxQueueSize)
					return;

				foreach (var loggingEvent in loggingEvents)
					_loggingEventQueue.Enqueue(loggingEvent);
			}
		}

		public LoggingEvent Dequeue()
		{
			return Dequeue(1).FirstOrDefault();
		}

		public LoggingEvent[] Dequeue(int count)
		{
			var dequeued = new List<LoggingEvent>();

			lock (_locker)
			{
				for (int i = 0; i < count; ++i)
				{
					if (_loggingEventQueue.Count <= 0)
						break;

					dequeued.Add(_loggingEventQueue.Dequeue());
				}
			}

			return dequeued.ToArray();
		}

		public void Clear()
		{
			lock (_locker)
			{
				_loggingEventQueue.Clear();
			}
		}
	}
}
