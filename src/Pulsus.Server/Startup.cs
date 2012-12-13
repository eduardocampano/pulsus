using System.Web.Http;
using System.Web.Routing;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Pulsus.Server.Startup), "Start")]
namespace Pulsus.Server
{
	public class Startup
	{
		public static void Start()
		{
			var config = GlobalConfiguration.Configuration;

			config.Routes.MapHttpRoute(
				name: "PulsusServerLog",
				routeTemplate: "api/log/",
				defaults: new { controller = "log" },
				constraints: new { httpMethod = new HttpMethodConstraint("GET", "POST") }
			);

			config.Formatters.Insert(0, new JsonpMediaTypeFormatter());
		}
	}
}
