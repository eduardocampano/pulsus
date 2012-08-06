using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Pulsus.Mvc;

[assembly: WebActivator.PreApplicationStartMethod(typeof(AppStart), "Start")]
namespace Pulsus.Mvc
{
	public static class AppStart
	{
		public static void Start()
		{
			DynamicModuleUtility.RegisterModule(typeof(ErrorLoggingModule));
			DynamicModuleUtility.RegisterModule(typeof(ErrorHandlingModule));

			LogManager.EventDispatcher = new DependencyResolverEventDispatcher();
			LogManager.EventFactory = new DependencyResolverEventFactory();
		}
	}
}
