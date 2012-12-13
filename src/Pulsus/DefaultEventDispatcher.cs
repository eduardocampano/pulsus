using System;
using System.Collections.Generic;
using System.Linq;
using Pulsus.Internal;
using Pulsus.Targets;

namespace Pulsus
{
	public class DefaultEventDispatcher : IEventDispatcher
	{
		private readonly IEnumerable<ITarget> _targets; 

		public DefaultEventDispatcher(IEnumerable<ITarget> targets = null)
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
					target.Push(loggingEvents);
				}
				catch (Exception ex)
				{
					// a target may fail but we need to continue with the others
					PulsusLogger.Error(ex);
				}
			}
		}

		protected virtual ITarget[] GetTargets()
		{
			return _targets.Where(t => t.Enabled).ToArray();
		}
	}
}
