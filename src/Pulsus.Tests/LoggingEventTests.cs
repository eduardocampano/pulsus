using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SharpTestsEx;

namespace Pulsus.Tests
{
	[TestFixture]
	public class LoggingEventTests
	{
		[Test]
		public void NullDataValueShouldReturnDefaultValue()
		{
			var loggingEvent = new LoggingEvent();
			loggingEvent.Data.Add("test", null);

			var value = loggingEvent.GetData<string>("test");

			value.Should().Be(null);
		}
	}
}
