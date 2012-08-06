using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace Pulsus.Sample.Controllers
{
	public class HomeController : Controller
	{
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public ActionResult Trace()
		{
			var stopwatch = Stopwatch.StartNew();

			for (var i = 1; i < 100; i++)
			{
				LogManager.EventFactory.Create()
									.Level(LoggingEventLevel.Trace)
									.Text("Trace No. {0}", i)
									.AddTags("trace")
									.AddStrackTrace()
									.Push();
			}

			stopwatch.Stop();

			return Content(string.Format("Elapsed: {0}", stopwatch.Elapsed));
		}

		[HttpGet]
		public ActionResult Error()
		{
			throw new NullReferenceException("Testing unhandled exception");
		}
	}
}
