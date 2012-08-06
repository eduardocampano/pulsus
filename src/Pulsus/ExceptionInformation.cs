using System;
using System.Reflection;
using System.Web;

namespace Pulsus
{
	public class ExceptionInformation
	{
		public static ExceptionInformation Create(Exception exception)
		{
			var exceptionInformation = new ExceptionInformation();
			exceptionInformation.Message = exception.Message;
			exceptionInformation.StackTrace = exception.StackTrace;
			exceptionInformation.Type = exception.GetType().Name.Replace("Exception", string.Empty);
			exceptionInformation.Source = exception.Source;

			var httpException = exception as HttpException;
			if (httpException != null)
			{
				exceptionInformation.StatusCode = httpException.GetHttpCode();

			}

			// TODO consider loader exception
			var typeLoadException = exception as TypeLoadException;
			if (typeLoadException != null)
			{ 
				//typeLoadException.TypeName
			}

			var reflectionTypeLoadException = exception as ReflectionTypeLoadException;
			if (reflectionTypeLoadException != null)
			{
				//reflectionTypeLoadException.LoaderExceptions;
			}

			return exceptionInformation;
		}

		public string Message { get; set; }
		public string StackTrace { get; set; }
		public string Type { get; set; }
		public string Source { get; set; }
		public int StatusCode { get; set; }
	}
}
