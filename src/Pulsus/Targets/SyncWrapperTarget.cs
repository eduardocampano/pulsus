using System.Web.Hosting;
using Pulsus.Internal;

namespace Pulsus.Targets
{
	public class SyncWrapperTarget : ITarget, IRegisteredObject
	{
        private readonly object _locker = new object();
        private bool _shuttingDown;
		protected readonly ITarget WrappedTarget;

		public SyncWrapperTarget(ITarget wrappedTarget)
        {
			WrappedTarget = wrappedTarget;

			if (HostingEnvironment.IsHosted)
				HostingEnvironment.RegisterObject(this);	
        }

		public virtual bool Enabled
		{
			get
			{
				return WrappedTarget.Enabled;
			}
		}

		public virtual void Push(LoggingEvent[] loggingEvents)
		{
			if (HostingEnvironment.IsHosted)
			{
				lock (_locker)
				{
					if (_shuttingDown)
						return;

					PushInternal(loggingEvents);
				}
			}
			else
			{
				PushInternal(loggingEvents);
			}
		}

		protected virtual void PushInternal(LoggingEvent[] loggingEvents)
		{
			PulsusLogger.Write("[{0}] Flushing {1} events to {2}", GetType().Name, loggingEvents.Length, WrappedTarget);
			WrappedTarget.Push(loggingEvents);
		}

	    public virtual void Stop(bool immediate)
	    {
            lock (_locker)
            {
                _shuttingDown = true;
            }

            HostingEnvironment.UnregisterObject(this);
	    }

		public override string ToString()
		{
			return string.Format("{0} wrapping ({1})", GetType().Name, WrappedTarget);
		}
	}
}
