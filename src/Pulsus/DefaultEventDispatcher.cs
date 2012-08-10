using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Pulsus
{
	public class DefaultEventDispatcher : IEventDispatcher
	{
		private delegate void PushDelegate(LoggingEvent loggingEvent);

		protected IList<ITarget> RegisteredTargets = new List<ITarget>();

        public IEnumerable<ITarget> DefaultTargets { get; set; }

		public virtual void Push(LoggingEvent loggingEvent, bool async)
		{
			if (async)
				new PushDelegate(PushInternal).BeginInvoke(loggingEvent, null, null);
			else
				PushInternal(loggingEvent);
		}

	    [DebuggerNonUserCode]
		protected void PushInternal(LoggingEvent loggingEvent)
		{
			var loggers = GetTargets().ToList();

            if (!loggers.Any())
                throw new InvalidOperationException("Cannot Push without Targets");

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
			RegisteredTargets.Add(logger);
		}

		protected virtual IEnumerable<ITarget> GetTargets()
		{
			var targets = new List<ITarget>();

			// add registered loggers
			targets.AddRange(RegisteredTargets);

			if (!LogManager.DisableDefaultTargets && targets.Count == 0 && DefaultTargets != null)
				targets.AddRange(DefaultTargets);

			return targets;
		}
	}
}
