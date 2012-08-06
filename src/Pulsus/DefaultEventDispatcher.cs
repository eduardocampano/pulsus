using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pulsus
{
	public class DefaultEventDispatcher : IEventDispatcher
	{
		private delegate void PushDelegate(LoggingEvent loggingEvent);

		protected IList<ITarget> registeredTargets = new List<ITarget>();

		public virtual void Push(LoggingEvent loggingEvent, bool async)
		{
			if (async)
				new PushDelegate(PushInternal).BeginInvoke(loggingEvent, null, null);
			else
				PushInternal(loggingEvent);
		}

		[DebuggerNonUserCode()]
		protected void PushInternal(LoggingEvent loggingEvent)
		{
			var loggers = GetTargets();
			foreach (var logger in loggers)
			{
				try
				{
					logger.Log(loggingEvent);
				}
				catch (Exception)
				{
					// TODO save logging event locally and push it 
				}
			}
		}

		public virtual void AddTarget(ITarget logger)
		{
			registeredTargets.Add(logger);
		}

		protected virtual IEnumerable<ITarget> GetTargets()
		{
			var targets = new List<ITarget>();

			// add registered loggers
			targets.AddRange(registeredTargets);

			if (!LogManager.DisableDefaultTargets && targets.Count == 0)
			{
				targets.Add(new MsSqlTarget());
				targets.Add(new RemoteTarget());
			}

			return targets;
		}
	}
}
