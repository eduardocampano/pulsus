using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Pulsus.Internal;

namespace Pulsus
{
	public class ExceptionInformation
	{
		public static ExceptionInformation Create(Exception exception)
		{
			// if it's not a .Net core exception, usually more information is being added
			// so use the wrapper for the message, type, etc.
			// if it's a .Net core exception type or an HttpUnhandledException, drill down and get the innermost exception
			if (exception.IsBuiltInException() || exception.IsHttpUnhandledException())
				exception = exception.GetBaseException();

			var exceptionInformation = new ExceptionInformation();
			exceptionInformation.Message = exception.Message;
			exceptionInformation.Type = exception.GetType().Name.Replace("Exception", string.Empty);
			exceptionInformation.Source = exception.Source;
			exceptionInformation.More = new List<ExceptionInformation>();

			var httpException = exception as HttpException;
			if (httpException != null)
				exceptionInformation.StatusCode = httpException.GetHttpCode();
			else
				exceptionInformation.StatusCode = 500;

			var reflectionTypeLoadException = exception as ReflectionTypeLoadException;
			if (reflectionTypeLoadException != null)
			{
				foreach (var ex in reflectionTypeLoadException.LoaderExceptions)
					exceptionInformation.More.Add(Create(ex));
			}

			return exceptionInformation;
		}

		public string Message { get; set; }
		public string Type { get; set; }
		public string Source { get; set; }
		public int StatusCode { get; set; }
		public IList<ExceptionInformation> More { get; set; }
	}
}
