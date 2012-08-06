using System;
using System.Web;

namespace Pulsus
{
	public static class LoggingEventBuilderExtensions
	{
		public static T AddException<T>(this T builder, Exception exception) where T : ILoggingEventBuilder<T>, new()
		{
			if (exception == null)
				return builder;

			var exceptionInformation = ExceptionInformation.Create(exception);

			if (string.IsNullOrEmpty(builder.LoggingEvent.Text))
				builder.Text(exceptionInformation.Message);

			builder.Level(LoggingEventLevel.Error);
			builder.AddTags("error", exceptionInformation.Type);
			builder.AddData("MS_Exception", exceptionInformation);

			return builder;
		}

		public static T AddHttpContext<T>(this T builder, HttpContext httpContext = null) where T : ILoggingEventBuilder<T>, new()
		{
			// if no HttpContext specified use the current context
			if (httpContext == null)
				httpContext = HttpContext.Current;

			// if no HttpContext yet then no HttpContext logging
			if (httpContext == null)
				return builder;

			var httpContextInformation = HttpContextInformation.Create(httpContext);

			if (string.IsNullOrEmpty(builder.LoggingEvent.User))
				builder.User(httpContextInformation.User);

			builder.AddData("MS_HttpContext", httpContextInformation);

			return builder;
		}

		public static T AddStrackTrace<T>(this T builder) where T : ILoggingEventBuilder<T>, new()
		{
			var stackTrace = StackTraceHelper.GetStackTrace();
			builder.AddData("MS_StackTrace", stackTrace.ToString());
			return builder;
		}
	}
}
