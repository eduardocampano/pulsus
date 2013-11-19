using System;
using System.Web;

namespace Pulsus.Internal
{
	internal static class ExceptionHelpers
	{
		public static bool IsBuiltInException(this Exception e)
		{
			return e.GetType().Module.ScopeName == "CommonLanguageRuntimeLibrary";
		}

	    public static bool IsHttpUnhandledException(this Exception e)
	    {
	        return e is HttpUnhandledException;
	    }
	}
}
