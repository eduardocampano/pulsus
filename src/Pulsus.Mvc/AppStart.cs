using Microsoft.Web.Infrastructure.DynamicModuleHelper;

namespace Pulsus.Mvc
{
	public static class AppStart
	{
		public static void Start()
		{
			DynamicModuleUtility.RegisterModule(typeof(ErrorLoggingModule));
		}
	}
}
