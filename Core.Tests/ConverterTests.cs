using System;
using NightlyCode.Core.Conversion;
using NUnit.Framework;

namespace GoorooMania.Core.Tests {

    [TestFixture]
    public class ConverterTests {

        [Test]
        public void TestLongToNullableDatetime() {
            DateTime? time = DateTime.Now;
            long ticks = time.Value.Ticks;
            DateTime? result = Converter.Convert<DateTime?>(ticks);
            Assert.AreEqual(time, result);
        }

        [Test]
        public void TestStringToTimespan() {
            TimeSpan timespan=Converter.Convert<TimeSpan>("30.00:00:00");
            Assert.AreEqual(TimeSpan.FromDays(30), timespan);
        }

        [Test]
        public void TestVersionToLong() {
            Version version = new Version(0, 9, 12345, 3233);
            long encoded = Converter.Convert<long>(version);
            Version decoded = Converter.Convert<Version>(encoded);
            Assert.AreEqual(version, decoded);

        }
    }
}