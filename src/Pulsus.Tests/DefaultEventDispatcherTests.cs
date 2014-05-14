using Moq;
using NUnit.Framework;
using Pulsus.Configuration;
using Pulsus.Targets;

namespace Pulsus.Tests
{
    [TestFixture]
    public class DefaultEventDispatcherTests
    {
        [Test]
        public void CanPushToTargets()
        {
            var target1Mock = new Mock<Target>();
            var target2Mock = new Mock<Target>();

            var configuration = new PulsusConfiguration();
            configuration.AddTarget("1", target1Mock.Object);
            configuration.AddTarget("2", target2Mock.Object);

            var eventDispatcher = new DefaultEventDispatcher(configuration);

            var loggingEvent = new LoggingEvent { Text = "Event" };
            var loggingEvents = new[] { loggingEvent };

            eventDispatcher.Push(loggingEvents);

            target1Mock.Verify(o => o.Push(loggingEvents));
            target2Mock.Verify(o => o.Push(loggingEvents));
        }
    }
}
