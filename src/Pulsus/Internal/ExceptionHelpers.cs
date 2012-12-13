using System;

namespace Pulsus.Internal
{
	internal static class ExceptionHelpers
	{
		public static bool IsBuiltInException(this Exception e)
		{
			return e.GetType().Module.ScopeName == "CommonLanguageRuntimeLibrary";
		}
	}
}
