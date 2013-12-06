using System;
using System.Web;
using Pulsus.Internal;

namespace Pulsus
{
    public static class LoggingEventBuilderExtensions
    {
        public static T AddException<T>(this T builder, Exception exception) where T : class, ILoggingEventBuilder<T>
        {
            return builder.AddException(exception, null);
        }

        public static T AddException<T>(this T builder, Exception exception, string source) where T : class, ILoggingEventBuilder<T>
        {
            if (exception == null)
                return builder;

            try
            {
                var exceptionInformation = ExceptionInformation.Create(exception, source);

                if (string.IsNullOrEmpty(builder.LoggingEvent.Text))
                    builder.Text(exceptionInformation.Message);

                builder.Level(LoggingEventLevel.Error);
                builder.AddTags("error", exceptionInformation.Type);

                builder.AddData(Constants.DataKeys.Exception, exceptionInformation);
                builder.AddData(Constants.DataKeys.StackTrace, exception.ToString());

                if (exception.Data.Contains("SQL"))
                {
                    var sql = exception.Data["SQL"] as string;
                    object sqlParameters = null;
                    if (exception.Data.Contains("SQLParam"))
                        sqlParameters = exception.Data["SQLParam"];

                    builder.AddSql(sql, sqlParameters);
                }

                // for exceptions we force including the HttpContext info
                if (!builder.LoggingEvent.Data.ContainsKey(Constants.DataKeys.HttpContext))
                    builder.AddHttpContext();
            }
            catch (Exception ex)
            {
                PulsusDebugger.Error(ex);
            }

            return builder;
        }

        public static T AddSql<T>(this T builder, string sql, object parameters = null) where T : class, ILoggingEventBuilder<T>
        {
            var sqlInfo = SqlInformation.Create(sql, parameters);
            builder.AddData(Constants.DataKeys.SQL, sqlInfo);
            return builder;
        }

        public static T AddHttpContext<T>(this T builder, HttpContext httpContext = null) where T : class, ILoggingEventBuilder<T>
        {
            // if no HttpContext specified use the current context
            if (httpContext == null)
                httpContext = HttpContext.Current;

            // if no HttpContext yet then no HttpContext logging
            if (httpContext == null)
                return builder;

            return builder.AddHttpContext(new HttpContextWrapper(httpContext));
        }

        public static T AddHttpContext<T>(this T builder, HttpContextBase httpContext) where T : class, ILoggingEventBuilder<T>
        {
            // if no HttpContext yet then no HttpContext logging
            if (httpContext == null)
                return builder;

            try
            {
                var httpContextInformation = HttpContextInformation.Create(httpContext);

                if (string.IsNullOrEmpty(builder.LoggingEvent.User))
                    builder.User(httpContextInformation.User);

                builder.AddData(Constants.DataKeys.HttpContext, httpContextInformation);
            }
            catch (Exception ex)
            {
                PulsusDebugger.Error(ex);
            }
           
            return builder;
        }

        public static T AddStrackTrace<T>(this T builder) where T : class, ILoggingEventBuilder<T>
        {
            try
            {
                var stackTrace = StackTraceHelper.GetStackTrace();
                builder.AddData(Constants.DataKeys.StackTrace, stackTrace);
            }
            catch (Exception ex)
            {
                PulsusDebugger.Error(ex);
            }
            
            return builder;
        }
    }
}
