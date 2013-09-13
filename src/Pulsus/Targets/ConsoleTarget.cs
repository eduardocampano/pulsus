using System;
using Pulsus.Internal;

namespace Pulsus.Targets
{
    public class ConsoleTarget : Target
    {
        public ConsoleTarget()
        {
            Format = "{LogKey} - {Text}";
        }

        public virtual string Format { get; set; }

        public override void Push(LoggingEvent[] loggingEvents)
        {
            foreach (var loggingEvent in loggingEvents)
            {
                Console.WriteLine(Format.Format(loggingEvent));
            }
        }
    }
}
