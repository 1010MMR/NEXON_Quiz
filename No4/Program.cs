using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Solution
{
    class NextPowerOfTwo
    {
        private const ulong MIN_INPUT_VALUE = 2;
        private const int MIN_INPUT_COUNT = 1;

        private int m_maxInputCount = 0;
        private ulong m_maxInputValue = 0;

        public NextPowerOfTwo()
        {
            m_maxInputCount = (int)Math.Pow(2, 18);
            m_maxInputValue = (ulong)Math.Pow(2, 63);
        }

        public int GetMaxInputCount(int value)
        {
            // return Math.Clamp(value, MIN_INPUT_COUNT, m_maxInputCount - 1);
            return (value <= MIN_INPUT_COUNT) ? MIN_INPUT_COUNT :
                   (value > m_maxInputCount) ? m_maxInputCount - 1 : value;
        }

        public ulong GetMaxInputValue(ulong value)
        {
            // return Math.Clamp(value, MIN_INPUT_VALUE, m_maxInputValue - 1);
            return (value < MIN_INPUT_VALUE) ? MIN_INPUT_VALUE :
                   (value > m_maxInputValue) ? m_maxInputValue - 1 : value;
        }

        public ulong MakeResult(ulong[] valueArray)
        {
            ulong result = ReturnNextPot(valueArray[0]);
            for (int i = 1; i < valueArray.Length; i++)
                result ^= ReturnNextPot(valueArray[i]);

            return result;
        }

        private ulong ReturnNextPot(ulong value)
        {
            --value;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
        }
    }

    class Solution
    {
        static void Main(string[] args)
        {
            NextPowerOfTwo npot = new NextPowerOfTwo();

            int inputCount = npot.GetMaxInputCount(int.Parse(Console.ReadLine()));
            ulong[] valueArray = new ulong[inputCount];

            for (int i = 0; i < inputCount; i++)
                valueArray[i] = npot.GetMaxInputValue(ulong.Parse(Console.ReadLine()));

            Console.Write(npot.MakeResult(valueArray));
        }
    }
}
