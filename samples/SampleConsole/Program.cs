using System;
using Pulsus;

namespace SampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter something to log and press return (enter nothing to exit)");

            var textToLog = Console.ReadLine();

            while (!string.IsNullOrEmpty(textToLog))
            {
                LogManager.EventFactory.Create()
                                   .Level(LoggingEventLevel.Trace)
                                   .AddTags("console")
                                   .Text(textToLog)
                                   .Push();

                textToLog = Console.ReadLine();
            }
        }
    }
}
