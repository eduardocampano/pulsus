using System;
using System.Linq;
using Pulsus.Configuration;
using Pulsus.Internal;
using Pulsus.Targets;

namespace Pulsus
{
    public class DefaultEventDispatcher : IEventDispatcher
    {
        private readonly PulsusConfiguration _configuration; 

        public DefaultEventDispatcher(PulsusConfiguration configuration)
        {
            _configuration = configuration;
        }

        public virtual void Push(LoggingEvent[] loggingEvents)
        {
            var targets = GetTargets();
            foreach (var target in targets)
            {
                try
                {
                    var loggingEventsToPush = loggingEvents.Where(x => CompliesTargetConditionsAndIgnores(x, target)).ToArray();

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

        protected virtual bool CompliesTargetConditionsAndIgnores(LoggingEvent loggingEvent, Target target)
        {
            if (loggingEvent == null)
                throw new ArgumentNullException("loggingEvent");

            if (target == null)
                throw new ArgumentNullException("target");

            if (!MatchesFilterConditions(loggingEvent, target))
                return false;

            var matchesAnyIgnoreFilter = target.Ignores != null && target.Ignores.Any(ignoreFilter => MatchesFilterConditions(loggingEvent, ignoreFilter));

            if (matchesAnyIgnoreFilter)
                return false;

            return true;
        }

        protected virtual bool MatchesFilterConditions(LoggingEvent loggingEvent, IFilter filter)
        {
            if (filter.MinLevel != LoggingEventLevel.None && loggingEvent.Level < filter.MinLevel)
                return false;

            if (filter.MaxLevel != LoggingEventLevel.None && loggingEvent.Level > filter.MaxLevel)
                return false;

            if (!filter.LogKeyContains.IsNullOrEmpty() && loggingEvent.LogKey.IndexOf(filter.LogKeyContains, StringComparison.OrdinalIgnoreCase) < 0)
                return false;

            if (!filter.LogKeyStartsWith.IsNullOrEmpty() && !loggingEvent.LogKey.StartsWith(filter.LogKeyStartsWith, StringComparison.OrdinalIgnoreCase))
                return false;

            if (!filter.TextContains.IsNullOrEmpty() && loggingEvent.Text.IndexOf(filter.TextContains, StringComparison.OrdinalIgnoreCase) < 0)
                return false;

            if (!filter.TextStartsWith.IsNullOrEmpty() && !loggingEvent.Text.StartsWith(filter.TextStartsWith, StringComparison.OrdinalIgnoreCase))
                return false;

            if (filter.MinValue.HasValue && loggingEvent.Value.HasValue && loggingEvent.Value.Value < filter.MinValue.Value)
                return false;

            if (filter.MaxValue.HasValue && loggingEvent.Value.HasValue && loggingEvent.Value.Value > filter.MaxValue.Value)
                return false;

            if (!filter.TagsContains.IsNullOrEmpty())
            {
                var requiredTags = TagHelpers.Clean(filter.TagsContains);
                if (!requiredTags.All(x => loggingEvent.Tags.Contains(x, StringComparer.OrdinalIgnoreCase)))
                    return false;
            }

            return true;
        }

        protected virtual Target[] GetTargets()
        {
            return _configuration.Targets.Values.ToArray();
        }
    }
}
