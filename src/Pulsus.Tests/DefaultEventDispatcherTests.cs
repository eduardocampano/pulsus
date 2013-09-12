using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
            var target1Mock = new Mock<Target>();
            var target2Mock = new Mock<Target>();

            var eventDispatcher = new DefaultEventDispatcher(new []{ target1Mock.Object, target2Mock.Object });

            var loggingEvent = new LoggingEvent { Text = "Event" };
            var loggingEvents = new[] { loggingEvent };

            eventDispatcher.Push(loggingEvents);

            target1Mock.Verify(o => o.Push(loggingEvents));
            target2Mock.Verify(o => o.Push(loggingEvents));
        }

        [Test]
        public void EventMatchingFiltersShouldBePushed()
        {
            var targetMock = new Mock<Target>();
            targetMock.SetupGet(x => x.MinLevel).Returns(LoggingEventLevel.Warning);
            targetMock.SetupGet(x => x.MaxLevel).Returns(LoggingEventLevel.Error);

            var loggingEvent = new LoggingEvent()
            {
                Level = (int)LoggingEventLevel.Error
            };

            var eventDispatcher = new DefaultEventDispatcher(new[] { targetMock.Object });
            eventDispatcher.Push(new[] { loggingEvent });

            targetMock.Verify(x => x.Push(It.IsAny<LoggingEvent[]>()), Times.Once);
        }

        [Test]
        public void EventNotMatchingFiltersShouldBeIgnored()
        {
            var targetMock = new Mock<Target>();
            targetMock.SetupGet(x => x.MinLevel).Returns(LoggingEventLevel.Warning);
            
            var loggingEvent = new LoggingEvent()
            {
                Level = (int)LoggingEventLevel.Information
            };

            var eventDispatcher = new DefaultEventDispatcher(new[] { targetMock.Object });
            eventDispatcher.Push(new[] { loggingEvent });

            targetMock.Verify(x => x.Push(It.IsAny<LoggingEvent[]>()), Times.Never);
        }

        [Test]
        public void EventMatchingIgnoreShouldBeIgnored()
        {
            var targetMock = new Mock<Target>();
            targetMock.SetupGet(x => x.Ignores).Returns(new Collection<Ignore>()
            {
                new Ignore()
                {
                    TagsContains = "tag2 tag3"
                }
            });

            var loggingEvent = new LoggingEvent()
            {
                Tags = new List<string>() { "tag1", "tag2", "tag3", "tag4" }
            };

            var eventDispatcher = new DefaultEventDispatcher(new[] { targetMock.Object });
            eventDispatcher.Push(new[] { loggingEvent });

            targetMock.Verify(x => x.Push(It.IsAny<LoggingEvent[]>()), Times.Never);
        }
    }
}
