using System;
using Pulsus;

namespace SampleConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			LogManager.Settings.LogKey = "Console";
			LogManager.Settings.Server.Enabled = true;
			LogManager.Settings.Server.Url = "http://localhost:5326/api/Log";
			LogManager.Settings.Server.ApiKey = "11111111";
			LogManager.Settings.Server.Compress = false;

			LogManager.EventFactory.Create()
								   .Level(LoggingEventLevel.Trace)
								   .AddTags("console")
								   .Text("Pushing from console")
								   .Push();

			Console.ReadKey();
		}
	}
}
