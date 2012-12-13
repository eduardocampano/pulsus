using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using Pulsus.Targets;

namespace Pulsus.Tests
{
	[TestFixture]
	public class AsyncWrapperTargetTests
	{
		[Test]
		public void WhenInternalTargetTakesMoreOrLessTheTimerIntervalIsAdjustedAutomatically()
		{
			var flag = new AutoResetEvent(false);
			var targetMock = new TargetMock();

			var asyncWrapperTarget = new AsyncWrapperTarget(targetMock);
			asyncWrapperTarget.TimerIntervalChanged += x => flag.Set();

			AssertTimerIntervalChanges(asyncWrapperTarget, targetMock, flag, 200);
			AssertTimerIntervalChanges(asyncWrapperTarget, targetMock, flag, 500);
			AssertTimerIntervalChanges(asyncWrapperTarget, targetMock, flag, 100);
		}

		private void AssertTimerIntervalChanges(AsyncWrapperTarget asyncWrapperTarget, TargetMock targetMock, AutoResetEvent flag, int internalTargetLatency)
		{
			var loggingEvents = new[] { new LoggingEvent() };
			targetMock.SleepFor = internalTargetLatency;
			asyncWrapperTarget.Push(loggingEvents);
			flag.WaitOne();
			Debug.Print("TimerInterval is {0} ms", asyncWrapperTarget.TimerInterval);
			var diff = Math.Abs(asyncWrapperTarget.TimerInterval - internalTargetLatency);
			Assert.LessOrEqual(diff, 10, "The TimerInterval was not adjusted properly.");
		}

		private class TargetMock : ITarget
		{
			public int SleepFor { get; set; }

			public bool Enabled
			{
				get { return true; }
			}

			public void Push(LoggingEvent[] loggingEvents)
			{
				Thread.Sleep(SleepFor);
			}
		}
	}
}
