using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Configuration;
using System.Configuration;

namespace Pulsus
{
	public class ErrorHandlingModule : IHttpModule
	{
		private static bool _useCustomErrorsDefinedInConfig;
		
		public static IList<int> StatusCodesToIgnoreFromWebConfig = new List<int>();
		public static CustomErrorsMode ErrorsMode;

		public void Init(HttpApplication context)
		{
			LoadCustomErrorsMode();

			//GlobalFilters.Filters.Add(new ErrorHandlerActionExecutedFilter());

			context.Error += (sender, e) =>
			{
				var exception = context.Server.GetLastError();

				if (ErrorHandlingModule.ErrorsMode == CustomErrorsMode.On || (ErrorHandlingModule.ErrorsMode == CustomErrorsMode.RemoteOnly && !context.Request.IsLocal))
				{	
					HandleException(context, exception);
				}
			};

			context.EndRequest += (sender, e) =>
			{
				if (ErrorHandlingModule.ErrorsMode == CustomErrorsMode.On || (ErrorHandlingModule.ErrorsMode == CustomErrorsMode.RemoteOnly && !context.Request.IsLocal))
				{
					//check for status codes that *aren't* errors or status codes that we skipped.
					//while IIS might throw a filenotfound error, manually returning an HttpNotFoundResult
					//does not generate an exception
					var exceptionThrown = context.Context.Items["ErrorHandlerExceptionThrown"] as bool?;
					if ((!exceptionThrown.HasValue || exceptionThrown.Value == false)
						&& ((context.Response.StatusCode >= 400
						&& context.Response.StatusCode < 600)
						&& !ErrorHandlingModule.StatusCodesToIgnoreFromWebConfig.Contains(context.Response.StatusCode)))
					{
						throw new HttpException(context.Response.StatusCode, context.Response.StatusDescription);
					}
				}
			};
		}

		public void Dispose() { }

		public static void HandleException(HttpApplication context, Exception exception)
		{
			if (exception == null)
				return;
			
			var httpException = exception as HttpException;
			int statusCode = httpException != null ? httpException.GetHttpCode() : 500;
			
			// check the statusCode against the config
			if (!StatusCodesToIgnoreFromWebConfig.Contains(statusCode))
			{
				context.Response.Clear();
				context.Server.ClearError();
				context.Response.StatusCode = statusCode;

				//var routeData = CreateRoute(context, exception);

				context.Response.TrySkipIisCustomErrors = true;

				//IController errorHandlerController = new ErrorHandlerController();
				//var rc = new RequestContext(new HttpContextWrapper(context.Context), routeData);
				//errorHandlerController.Execute(rc);
			}
		}

		public static void UseCustomErrorsDefinedInConfig()
		{
			_useCustomErrorsDefinedInConfig = true;
			LoadCustomErrorsMode();
		}

		private static void LoadCustomErrorsMode()
		{
			// ~ value open default web.config in current web application
			var config = WebConfigurationManager.OpenWebConfiguration("~/");
			var section = (CustomErrorsSection)config.GetSection("system.web/customErrors");
			ErrorsMode = section.Mode;

			if (_useCustomErrorsDefinedInConfig)
			{
				StatusCodesToIgnoreFromWebConfig.Clear();

				foreach (CustomError error in section.Errors)
				{
					//load the error code
					int statusCode = error.StatusCode;
					StatusCodesToIgnoreFromWebConfig.Add(statusCode);
				}
			}
		}
	}
}
