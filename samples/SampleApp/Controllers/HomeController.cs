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
            var amount = 2;

            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < amount; i++)
			{
				LogManager.EventFactory.Create()
									.Level(LoggingEventLevel.Trace)
									.Text("Trace No. {0}", i + 1)
									.AddTags("trace")
									.AddStrackTrace()
									.Push();
			}

			stopwatch.Stop();

			return Content(string.Format("Traced {0} events in {1} ms.", amount, stopwatch.ElapsedMilliseconds));
		}

		[HttpGet]
		public ActionResult Test1()
		{
			LogManager.EventFactory.Create()
					.Level(LoggingEventLevel.Trace)
					.Text("Testing Pulsus Event Push")
					.User("Eduardo")
					.AddTags("tag1 tag2")
					.AddData("Additional", new { a = 1, b = DateTime.UtcNow, c = "hello" })
					.AddStrackTrace()
					.Push();

			return Content("OK");
		}

		[HttpGet]
		public ActionResult Error()
		{
			throw new NullReferenceException("Testing unhandled exception");
		}
	}
}
