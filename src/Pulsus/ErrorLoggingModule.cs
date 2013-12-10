using System;
using System.Web;

namespace Pulsus
{
	public class ErrorLoggingModule : IHttpModule
	{
		public virtual void Init(HttpApplication context)
		{
			context.Error += (sender, e) =>
			{
				var exception = context.Server.GetLastError();

				LogException(context, exception);
			};
		}

		public virtual void Dispose() { }

		protected virtual void LogException(HttpApplication context, Exception exception)
		{
			if (exception == null)
				return;

			if (!ShouldLogException(context, exception))
				return;

			LogManager.EventFactory.Create()
								.AddException(exception, "ErrorLoggingModule")
								.Push();
		}

		protected virtual bool ShouldLogException(HttpApplication context, Exception exception)
		{
			foreach (var evaluator in LogManager.Configuration.ExceptionsToIgnore.Values)
			{
				if (evaluator == null)
					continue;

				if (evaluator(exception))
					return false;
			}

			return true;
		}
	}
}
