using System;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace Pulsus.Tests
{
    [TestFixture]
    public class DefaultEventDispatcherTests
    {
        [Test]
        public void CanAddAndPushToTargets()
        {
            var target1Mock = new Mock<ITarget>();
            var target2Mock = new Mock<ITarget>();

            var eventDispatcher = new DefaultEventDispatcher();

            eventDispatcher.AddTarget(target1Mock.Object);
            eventDispatcher.AddTarget(target2Mock.Object);

            var loggingEvent = new LoggingEvent { Text = "Event" };
            eventDispatcher.Push(loggingEvent, false);

            target1Mock.Verify(o => o.Log(loggingEvent));
            target2Mock.Verify(o => o.Log(loggingEvent));
        }

        [Test]
        public void CanPushToDefaultTargets()
        {
            var target1Mock = new Mock<ITarget>();
            var target2Mock = new Mock<ITarget>();

            var eventDispatcher = new DefaultEventDispatcher { DefaultTargets = new[] { target1Mock.Object, target2Mock.Object } };

            var loggingEvent = new LoggingEvent { Text = "Event" };
            eventDispatcher.Push(loggingEvent, false);

            target1Mock.Verify(o => o.Log(loggingEvent));
            target2Mock.Verify(o => o.Log(loggingEvent));
        }

        [Test]
        public void PushingWithNoTargetsShouldThrow()
        {
            var eventDispatcher = new DefaultEventDispatcher();

            var loggingEvent = new LoggingEvent { Text = "Event" };
            Executing.This(() => eventDispatcher.Push(loggingEvent, false)).Should().Throw<InvalidOperationException>();

        }
    }
}
