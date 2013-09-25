using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
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

            value.Should().Be.Null();
        }

        [Test]
        public void NonSerializedDataShouldCorrectlyRetrieved()
        {
            var loggingEvent = new LoggingEvent();
            loggingEvent.Data["custom-data"] = new CustomData()
            {
                StringField = "test",
                IntField = 9
            };

            var result = loggingEvent.GetData<CustomData>("custom-data");
            result.Should().Not.Be.Null();
            result.StringField.Should().Be("test");
            result.IntField.Should().Be(9);
        }

        [Test]
        public void SerializedDataShoudBeCorrectlyDeserialized()
        {
            const string serializedData = "{ \"custom-data\" : { \"StringField\" : \"test\", \"IntField\" : 9 } }";
            var loggingEvent = new LoggingEvent();
            loggingEvent.Data = JsonConvert.DeserializeObject<IDictionary<string, object>>(serializedData);

            var result = loggingEvent.GetData<CustomData>("custom-data");
            result.Should().Not.Be.Null();
            result.StringField.Should().Be("test");
            result.IntField.Should().Be(9);
        }

        private class CustomData
        {
            public string StringField { get; set; }
            public int IntField { get; set; }
        }
    }
}
