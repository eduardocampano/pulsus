using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using Pulsus.Configuration;
using Pulsus.Targets;
using System.Collections.Generic;

namespace Pulsus.Tests
{
    [TestFixture]
    public class TargetFiltersAndIgnoresTests
    {
        [Test]
        public void Event_Matching_MinLevel_Ignore_Filter_Should_Be_Ignored()
        {
            var targetMock = new Mock<Target>();
            targetMock.SetupGet(x => x.Ignores).Returns(new List<Ignore>()
            {
                new Ignore() { MinLevel = LoggingEventLevel.Warning }
            });

            var loggingEvent = new LoggingEvent()
            {
                Level = LoggingEventLevel.Error
            };

            var configuration = new PulsusConfiguration();
            configuration.AddTarget("1", targetMock.Object);

            var eventDispatcher = new DefaultEventDispatcher(configuration);
            eventDispatcher.Push(new[] { loggingEvent });

            targetMock.Verify(x => x.Push(It.IsAny<LoggingEvent[]>()), Times.Never);
        }

        [Test]
        public void Event_Matching_MaxLevel_Ignore_Filter_Should_Be_Ignored()
        {
            var targetMock = new Mock<Target>();
            targetMock.SetupGet(x => x.Ignores).Returns(new List<Ignore>()
            {
                new Ignore() { MaxLevel = LoggingEventLevel.Warning }
            });

            var loggingEvent = new LoggingEvent()
            {
                Level = LoggingEventLevel.Debug
            };

            var configuration = new PulsusConfiguration();
            configuration.AddTarget("1", targetMock.Object);

            var eventDispatcher = new DefaultEventDispatcher(configuration);
            eventDispatcher.Push(new[] { loggingEvent });

            targetMock.Verify(x => x.Push(It.IsAny<LoggingEvent[]>()), Times.Never);
        }

        [Test]
        public void Event_Matching_TextContains_Ignore_Filter_Should_Be_Ignored()
        {
            var targetMock = new Mock<Target>();
            targetMock.SetupGet(x => x.Ignores).Returns(new List<Ignore>()
            {
                new Ignore() { TextContains = "description" }
            });

            var loggingEvent = new LoggingEvent()
            {
                Text = "A description of the error"
            };

            var configuration = new PulsusConfiguration();
            configuration.AddTarget("1", targetMock.Object);

            var eventDispatcher = new DefaultEventDispatcher(configuration);
            eventDispatcher.Push(new[] { loggingEvent });

            targetMock.Verify(x => x.Push(It.IsAny<LoggingEvent[]>()), Times.Never);
        }

        [Test]
        public void Event_Matching_TextStartsWith_Ignore_Filter_Should_Be_Ignored()
        {
            var targetMock = new Mock<Target>();
            targetMock.SetupGet(x => x.Ignores).Returns(new List<Ignore>()
            {
                new Ignore() { TextStartsWith = "description" }
            });

            var loggingEvent = new LoggingEvent()
            {
                Text = "Description of the error"
            };

            var configuration = new PulsusConfiguration();
            configuration.AddTarget("1", targetMock.Object);

            var eventDispatcher = new DefaultEventDispatcher(configuration);
            eventDispatcher.Push(new[] { loggingEvent });

            targetMock.Verify(x => x.Push(It.IsAny<LoggingEvent[]>()), Times.Never);
        }

        [Test]
        public void Event_Matching_LogKeyContains_Ignore_Filter_Should_Be_Ignored()
        {
            var targetMock = new Mock<Target>();
            targetMock.SetupGet(x => x.Ignores).Returns(new List<Ignore>()
            {
                new Ignore() { LogKeyContains = "Sample.LogKey" }
            });

            var loggingEvent = new LoggingEvent()
            {
                LogKey = "Sample.LogKey"
            };

            var configuration = new PulsusConfiguration();
            configuration.AddTarget("1", targetMock.Object);

            var eventDispatcher = new DefaultEventDispatcher(configuration);
            eventDispatcher.Push(new[] { loggingEvent });

            targetMock.Verify(x => x.Push(It.IsAny<LoggingEvent[]>()), Times.Never);
        }

        [Test]
        public void Event_Matching_LogKeyStartsWith_Ignore_Filter_Should_Be_Ignored()
        {
            var targetMock = new Mock<Target>();
            targetMock.SetupGet(x => x.Ignores).Returns(new List<Ignore>()
            {
                new Ignore() { LogKeyStartsWith = "Sample." }
            });

            var loggingEvent = new LoggingEvent()
            {
                LogKey = "Sample.LogKey"
            };

            var configuration = new PulsusConfiguration();
            configuration.AddTarget("1", targetMock.Object);

            var eventDispatcher = new DefaultEventDispatcher(configuration);
            eventDispatcher.Push(new[] { loggingEvent });

            targetMock.Verify(x => x.Push(It.IsAny<LoggingEvent[]>()), Times.Never);
        }

        [Test]
        public void Event_Matching_MinValue_Ignore_Filter_Should_Be_Ignored()
        {
            var targetMock = new Mock<Target>();
            targetMock.SetupGet(x => x.Ignores).Returns(new List<Ignore>()
            {
                new Ignore() { MinValue = 1 }
            });

            var loggingEvent = new LoggingEvent()
            {
                Value = 2
            };

            var configuration = new PulsusConfiguration();
            configuration.AddTarget("1", targetMock.Object);

            var eventDispatcher = new DefaultEventDispatcher(configuration);
            eventDispatcher.Push(new[] { loggingEvent });

            targetMock.Verify(x => x.Push(It.IsAny<LoggingEvent[]>()), Times.Never);
        }

        [Test]
        public void Event_Matching_MaxValue_Ignore_Filter_Should_Be_Ignored()
        {
            var targetMock = new Mock<Target>();
            targetMock.SetupGet(x => x.Ignores).Returns(new List<Ignore>()
            {
                new Ignore() { MaxValue = 2 }
            });

            var loggingEvent = new LoggingEvent()
            {
                Value = 1
            };

            var configuration = new PulsusConfiguration();
            configuration.AddTarget("1", targetMock.Object);

            var eventDispatcher = new DefaultEventDispatcher(configuration);
            eventDispatcher.Push(new[] { loggingEvent });

            targetMock.Verify(x => x.Push(It.IsAny<LoggingEvent[]>()), Times.Never);
        }

        [Test]
        public void Event_Matching_TagContains_Ignore_Should_Be_Ignored()
        {
            var targetMock = new Mock<Target>();
            targetMock.SetupGet(x => x.Ignores).Returns(new List<Ignore>()
            {
                new Ignore() { TagsContains = "tag2" }
            });

            var loggingEvent = new LoggingEvent()
            {
                Tags = new List<string>() { "tag1", "tag2", "tag3", "tag4" }
            };

            var configuration = new PulsusConfiguration();
            configuration.AddTarget("1", targetMock.Object);

            var eventDispatcher = new DefaultEventDispatcher(configuration);
            eventDispatcher.Push(new[] { loggingEvent });

            targetMock.Verify(x => x.Push(It.IsAny<LoggingEvent[]>()), Times.Never);
        }

        [Test]
        public void Event_Matching_Multiple_TagContains_Ignore_Should_Be_Ignored()
        {
            var targetMock = new Mock<Target>();
            targetMock.SetupGet(x => x.Ignores).Returns(new List<Ignore>()
            {
                new Ignore() { TagsContains = "tag2 tag3" }
            });

            var loggingEvent = new LoggingEvent()
            {
                Tags = new List<string>() { "tag1", "tag2", "tag3", "tag4" }
            };

            var configuration = new PulsusConfiguration();
            configuration.AddTarget("1", targetMock.Object);

            var eventDispatcher = new DefaultEventDispatcher(configuration);
            eventDispatcher.Push(new[] { loggingEvent });

            targetMock.Verify(x => x.Push(It.IsAny<LoggingEvent[]>()), Times.Never);
        }

        [Test]
        public void Event_Complying_With_Level_Filters_Should_Be_Pushed()
        {
            var targetMock = new Mock<Target>();
            targetMock.SetupGet(x => x.MinLevel).Returns(LoggingEventLevel.Warning);
            targetMock.SetupGet(x => x.MaxLevel).Returns(LoggingEventLevel.Error);

            var loggingEvent = new LoggingEvent()
            {
                Level = LoggingEventLevel.Error
            };

            var configuration = new PulsusConfiguration();
            configuration.AddTarget("1", targetMock.Object);

            var eventDispatcher = new DefaultEventDispatcher(configuration);
            eventDispatcher.Push(new[] { loggingEvent });

            targetMock.Verify(x => x.Push(It.IsAny<LoggingEvent[]>()), Times.Once);
        }

        [Test]
        public void Event_Not_Complying_With_Level_Filters_Should_Be_Ignored()
        {
            var targetMock = new Mock<Target>();
            targetMock.SetupGet(x => x.MinLevel).Returns(LoggingEventLevel.Warning);

            var loggingEvent = new LoggingEvent()
            {
                Level = LoggingEventLevel.Information
            };

            var configuration = new PulsusConfiguration();
            configuration.AddTarget("1", targetMock.Object);

            var eventDispatcher = new DefaultEventDispatcher(configuration);
            eventDispatcher.Push(new[] { loggingEvent });

            targetMock.Verify(x => x.Push(It.IsAny<LoggingEvent[]>()), Times.Never);
        }
    }
}
