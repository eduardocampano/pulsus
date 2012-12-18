using System;
using System.ComponentModel;

namespace Pulsus.Targets
{
	public abstract class Target : IDisposable
	{
		public virtual string Name
		{
			get
			{
				return GetType().Name;
			}
		}

		[DefaultValue(true)]
		public virtual bool Enabled { get; set; }
		
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
