using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Solution
{
    class Cdc
    {
        private const ulong MIN_TIME_VALUE = 1;

        private const int MIN_PROP_VALUE = 1;
        private const int MAX_PROP_VALUE = 100;

        private ulong m_maxTimeValue = 0;
        private ulong m_retailValue = 0;

        public Cdc()
        {
            m_maxTimeValue = (ulong)Math.Pow(10, 19);
            m_retailValue = (ulong)Math.Pow(10, 9) + 7;
        }

        public int GetClampPropValue(int value)
        {
            // return Math.Clamp(value, MIN_MAN_VALUE, MAX_MAN_VALUE);
            return (value <= MIN_PROP_VALUE) ? MIN_PROP_VALUE :
                   (value >= MAX_PROP_VALUE) ? MAX_PROP_VALUE : value;
        }

        public ulong GetClampTimeValue(ulong value)
        {
            // return Math.Clamp(value, MIN_TIME_VALUE, m_maxTimeValue);
            return (value <= MIN_TIME_VALUE) ? MIN_TIME_VALUE :
                   (value >= m_maxTimeValue) ? m_maxTimeValue : value;
        }

        public void MakeResult(string readLine)
        {
            int man = 0; 
            ulong multi = 0;
            ulong time = 0;

            if (ParseValue(readLine, out man, out multi, out time))
            {
                string binary = ReturnBinary(time);
                int btLenght = binary.Length;

                ulong[] modArray = new ulong[btLenght];
                ulong retail = 0;

                for (int i = 0; i < btLenght; i++)
                {
                    if (i.Equals(0)) modArray[i] = ReturnRetailMod(multi);
                    else modArray[i] = ReturnRetailMod(modArray[i - 1] * modArray[i - 1]);

                    if (binary[btLenght - 1 - i].Equals('1'))
                    {
                        if (retail.Equals(0)) retail = modArray[i];
                        else retail *= modArray[i];

                        retail = ReturnRetailMod(retail);
                    }
                }

                Console.WriteLine(ReturnRetailMod((ulong)man * ReturnRetailMod(retail)));
            }
        }

        private bool ParseValue(string readLine, out int man, out ulong multi, out ulong time)
        {
            man = 0;
            multi = 0;
            time = 0;

            string[] split = readLine.Split(' ');
            if (split.Length.Equals(3))
            {
                man = GetClampPropValue(int.Parse(split[0]));
                multi = (ulong)GetClampPropValue(int.Parse(split[1]));
                time = GetClampTimeValue(ulong.Parse(split[2]));

                return true;
            }

            return false;
        }

        private string ReturnBinary(ulong time)
        {
            return Convert.ToString((long)time, 2);
        }

        private ulong ReturnRetailMod(double value)
        {
            return (ulong)(value - Math.Floor(value / m_retailValue) * m_retailValue);
        }
    }

    class Solution
    {
        static void Main(string[] args)
        {
            Cdc cdc = new Cdc();
            cdc.MakeResult(Console.ReadLine());
        }
    }
}