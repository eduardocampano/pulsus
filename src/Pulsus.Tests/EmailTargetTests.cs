using System;
using System.Net.Mail;
using NUnit.Framework;
using Pulsus.Targets;
using SharpTestsEx;

namespace Pulsus.Tests
{
    [TestFixture]
    public class EmailTargetTests
    {
        [Test]
        public void ThrottlingMustBeActivatedWhenThresholdIsReached()
        {
            var loggingEvent = new LoggingEvent()
            {
                Text = "Test"
            };

            var emailTarget = new EmailTargetTester();
            emailTarget.ThrottlingWindow = 1;
            emailTarget.ThrottlingThreshold = 60;
            emailTarget.ThrottlingDuration = 10;

            for (var i = 0; i < 100; i++)
                emailTarget.Push(new[] { loggingEvent });

            emailTarget.Counter.Should().Be(60);
        }
    }

    public class EmailTargetTester : EmailTarget
    {
        public int Counter { get; private set; }

        protected override MailMessage PrepareEmail(LoggingEvent loggingEvent)
        {
            return new MailMessage();
        }

        protected override void Send(MailMessage mailMessage)
        {
            Counter += 1;
            if (Counter > ThrottlingThreshold)
                throw new InvalidOperationException("The counter should not pass the threshold");
        }
    }
}
