using NightlyCode.Core.Randoms;
using NUnit.Framework;

namespace GoorooMania.Core.Tests {

    [TestFixture]
    public class RNGTests {

        [Test]
        public void Distribution() {
            RNG rng = new XORShift64RNG();
            for(int i = 0; i < 10000; ++i) {
                float value = rng.NextFloat();
            }
        }
    }
}