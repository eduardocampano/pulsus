using Microsoft.Web.Infrastructure.DynamicModuleHelper;

namespace Pulsus.Web
{
	public static class AppStart
	{
		public static void Start()
		{
			DynamicModuleUtility.RegisterModule(typeof(ErrorLoggingModule));
		}
	}
}
