using System.Web.Hosting;
using Pulsus.Internal;

namespace Pulsus.Targets
{
	public class WrapperTarget : Target, IRegisteredObject
	{
        private readonly object _locker = new object();
        private bool _shuttingDown;
		protected readonly Target WrappedTarget;

		public WrapperTarget(Target wrappedTarget)
        {
			WrappedTarget = wrappedTarget;

			if (HostingEnvironment.IsHosted)
				HostingEnvironment.RegisterObject(this);	
        }

		public override bool Enabled
		{
			get
			{
				return WrappedTarget.Enabled;
			}

			set
			{
				WrappedTarget.Enabled = value;
			}
		}

		public override void Push(LoggingEvent[] loggingEvents)
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
	}
}
