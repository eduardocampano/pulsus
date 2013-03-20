using System;
using System.Collections.Generic;
using System.Linq;
using Pulsus.Internal;
using Pulsus.Targets;

namespace Pulsus
{
	public class DefaultEventDispatcher : IEventDispatcher
	{
		private readonly IEnumerable<Target> _targets; 

		public DefaultEventDispatcher(IEnumerable<Target> targets = null)
		{
			_targets = targets;
		}

		public virtual void Push(LoggingEvent[] loggingEvents)
		{
			var targets = GetTargets();
			foreach (var target in targets)
			{
				try
				{
				    var loggingEventsToPush = loggingEvents.Where(x => MustPushToTarget(x, target)).ToArray();
                    target.Push(loggingEventsToPush);
				}
				catch (Exception ex)
				{
                    if (LogManager.Configuration.ThrowExceptions)
                        throw;

					// a target may fail but we need to continue with the others
					PulsusLogger.Error(ex);
				}
			}
		}

	    protected virtual bool MustPushToTarget(LoggingEvent loggingEvent, Target target)
	    {
            if (target.MinLevel > 0 && loggingEvent.Level < (int)target.MinLevel)
                return false;

            if (target.MaxLevel > 0 && loggingEvent.Level > (int)target.MaxLevel)
                return false;

	        return true;
	    }

	    protected virtual Target[] GetTargets()
		{
			return _targets.ToArray();
		}
	}
}
