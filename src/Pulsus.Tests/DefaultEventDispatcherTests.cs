using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Pulsus.Targets;
using SharpTestsEx;

namespace Pulsus.Tests
{
    [TestFixture]
    public class DefaultEventDispatcherTests
    {
        [Test]
        public void CanPushToTargets()
        {
            var target1Mock = new Mock<ITarget>();
            var target2Mock = new Mock<ITarget>();

			target1Mock.SetupGet(x => x.Enabled).Returns(true);
			target2Mock.SetupGet(x => x.Enabled).Returns(true);

            var eventDispatcher = new DefaultEventDispatcher(new List<ITarget>{ target1Mock.Object, target2Mock.Object });

            var loggingEvent = new LoggingEvent { Text = "Event" };
			var loggingEvents = new[] { loggingEvent };

			eventDispatcher.Push(loggingEvents);

			target1Mock.Verify(o => o.Push(loggingEvents));
			target2Mock.Verify(o => o.Push(loggingEvents));
        }
    }
}
