using System;

namespace NightlyCode.Core.Randoms {

    /// <summary>
    /// rng using xor shift operations
    /// </summary>
    /// <remarks>
    /// can not produce 0
    /// </remarks>
    public class XORShift64RNG : RNG {
        long value;

        public XORShift64RNG()
            : this(Environment.TickCount) {}


        /// <summary>
        /// creates a new xor shift 64 rng using the specified seed
        /// </summary>
        /// <param name="seed">seed to use</param>
        public XORShift64RNG(long seed)
        {
            value = seed;
            if (value == 0)
                // 0 is a degenerate seed leading to all future numbers 0
                // initialize this special case with some default seed
                value = 88172645463325252L;
        }

        public override long NextLong()
        {
            value ^= value << 21;
            value ^= (value >> 35) | (value << 29);
            value ^= value << 4;
            return value;
        }

    }
}