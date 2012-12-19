using System;
using Pulsus;
using Pulsus.Targets;

namespace SampleConsole
{
	class Program
	{
		static void Main(string[] args)
		{

			LogManager.Configuration.LogKey = "Console";
			//LogManager.Configuration.Targets.Add("server", new AsyncWrapperTarget(new ServerTarget()
			//																{
			//																	Enabled = true,
			//																	Url = "http://localhost:5326/api/Log",
			//																	ApiKey = "11111111",
			//																	Compress = false
			//																}));

			LogManager.Configuration.Targets.Add("amazonS3", new AmazonS3Target()
			{
				Enabled = true,
				AccessKey = "0RZGAEJM2Z0C2J4C0382",
				SecretKey = "pSBAkhCZqdivtWudMGLk57/gfbBuxDuWIl1fI/8m",
				BucketName = "ibe.oneworld.dev"
			});

			LogManager.EventFactory.Create()
								   .Level(LoggingEventLevel.Trace)
								   .AddTags("console")
								   .Text("Pushing from console")
								   .Push();

			Console.ReadKey();
		}
	}
}
