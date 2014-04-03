using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Pulsus.Internal;
using SharpTestsEx;

namespace Pulsus.Tests
{
    [TestFixture]
    public class LoggingEventBuilderTests
    {
        [Test]
        public void AddingTextShouldStoreTextInLoggingEvent()
        {
            var loggingEventMock = new Mock<LoggingEvent>();
            var loggingEventBuilder = new LoggingEventBuilder(loggingEventMock.Object);

            loggingEventMock.SetupSet(x => x.Text = It.Is<string>(v => v == "sometext"));

            loggingEventBuilder.Text("sometext");

            loggingEventMock.VerifyAll();
        }

        [Test]
        public void AddingExceptionShouldStoreExceptionInLoggingEvent()
        {
            var loggingEventMock = new Mock<LoggingEvent>();
            loggingEventMock.Object.Tags = new List<string>();

            var loggingEventBuilder = new LoggingEventBuilder(loggingEventMock.Object);

            loggingEventMock.SetupSet(x => x.Text = It.Is<string>(v => v == "exceptionmessage"));
            loggingEventMock.Setup(x => x.Tags.Add(It.Is<string>(v => v == "error")));
            loggingEventMock.SetupSet(x => x.Data["MS_Exception"] = It.IsAny<ExceptionInformation>());

            loggingEventBuilder.AddException(new Exception("exceptionmessage"));

            loggingEventMock.VerifyAll();
        }
    }
}
