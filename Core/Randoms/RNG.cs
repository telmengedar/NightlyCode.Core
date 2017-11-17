using NightlyCode.Core.ComponentModel;

namespace NightlyCode.Core.Randoms {

    /// <summary>
    /// base class for random number generators
    /// </summary>
    public abstract class RNG {
        static readonly InstanceProvider<RNG> instance = new InstanceProvider<RNG>(() => new XORShift64RNG());
        
        /// <summary>
        /// xor shift 64 implementation
        /// </summary>
        public static RNG XORShift64 => instance.Instance;

        public abstract long NextLong();

        public int NextInt() {
            return (int)NextLong();
        }

        public int NextInt(int max) {
            int value = NextInt() % max;
            if(value < 0) value += max;
            return value;
        }

        public short NextShort() {
            return (short)NextLong();
        }

        public byte NextByte() {
            return (byte)NextLong();
        }

        public float NextFloat() {
            return (float)System.Math.Abs(NextLong()) / long.MaxValue;
        }

        public float NextFloatRange() {
            return (float)NextLong() / long.MaxValue;
        }

        public double NextDouble() {
            return (double)System.Math.Abs(NextLong()) / long.MaxValue;
        }

    }
}