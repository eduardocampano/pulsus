using System;
using System.Collections.Generic;

namespace Pulsus.Targets
{
    public abstract class Target : IFilter, IDisposable
    {
        protected Target()
        {
            Ignores = new List<Ignore>();
        }

        public virtual string Name
        {
            get
            {
                return GetType().Name;
            }
        }

        public virtual bool Async { get; set; }

        public virtual ICollection<Ignore> Ignores { get; private set; }
        
        public virtual LoggingEventLevel MinLevel { get; set; }
        public virtual LoggingEventLevel MaxLevel { get; set; }
        public virtual string LogKeyContains { get; set; }
        public virtual string LogKeyStartsWith { get; set; }
        public virtual string TextContains { get; set; }
        public virtual string TextStartsWith { get; set; }
        public virtual string TagsContains { get; set; }
        public virtual double? MinValue { get; set; }
        public virtual double? MaxValue { get; set; }

        public abstract void Push(LoggingEvent[] loggingEvents);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}
