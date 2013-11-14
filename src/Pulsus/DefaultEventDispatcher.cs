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
                    var loggingEventsToPush = loggingEvents.Where(x => CheckTargetConditionsAndIgnores(x, target)).ToArray();

                    if (loggingEventsToPush.Length > 0)
                        target.Push(loggingEventsToPush);
                }
                catch (Exception ex)
                {
                    if (LogManager.Configuration.ThrowExceptions)
                        throw;

                    // a target may fail but we need to continue with the others
                    PulsusDebugger.Error(target, ex);
                }
            }
        }

        protected virtual bool CheckTargetConditionsAndIgnores(LoggingEvent loggingEvent, Target target)
        {
            if (loggingEvent == null)
                throw new ArgumentNullException("loggingEvent");

            if (target == null)
                throw new ArgumentNullException("target");

            if (!MatchesFilterConditions(loggingEvent, target))
                return false;

            if (target.Ignores == null || target.Ignores.Count == 0)
                return true;

            return !target.Ignores.Any(ignoreFilter => MatchesFilterConditions(loggingEvent, ignoreFilter));
        }

        protected virtual bool MatchesFilterConditions(LoggingEvent loggingEvent, IFilter filter)
        {
            if (filter.MinLevel != LoggingEventLevel.None && loggingEvent.Level < filter.MinLevel)
                return false;

            if (filter.MaxLevel != LoggingEventLevel.None && loggingEvent.Level > filter.MaxLevel)
                return false;

            if (!filter.LogKeyContains.IsNullOrEmpty() && loggingEvent.LogKey.Contains(filter.LogKeyContains))
                return false;

            if (!filter.LogKeyStartsWith.IsNullOrEmpty() && loggingEvent.LogKey.StartsWith(filter.LogKeyStartsWith))
                return false;

            if (!filter.TextContains.IsNullOrEmpty() && loggingEvent.Text.Contains(filter.TextContains))
                return false;

            if (!filter.TextStartsWith.IsNullOrEmpty() && loggingEvent.Text.StartsWith(filter.TextStartsWith))
                return false;

            if (filter.MinValue.HasValue && loggingEvent.Value.HasValue && loggingEvent.Value.Value < filter.MinValue.Value)
                return false;

            if (filter.MaxValue.HasValue && loggingEvent.Value.HasValue && loggingEvent.Value.Value < filter.MaxValue.Value)
                return false;

            if (!filter.TagsContains.IsNullOrEmpty())
            {
                var requiredTags = TagHelpers.Clean(filter.TagsContains);
                if (!requiredTags.All(x => loggingEvent.Tags.Contains(x)))
                    return false;
            }

            return true;
        }

        protected virtual Target[] GetTargets()
        {
            return _targets.ToArray();
        }
    }
}
