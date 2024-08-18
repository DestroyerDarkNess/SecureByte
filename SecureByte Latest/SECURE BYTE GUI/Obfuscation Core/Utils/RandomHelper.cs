using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICore
{
    public class RandomHelper
    {
        private int inext;
        private int inextp;
        private int[] SeedArray = new int[56];
        private const int MBIG = Int32.MaxValue;
        private const int MSEED = 161803398;
        private const int MZ = 0;
        public RandomHelper(int Seed)
        {
            int num = (Seed == int.MinValue) ? int.MaxValue : Math.Abs(Seed);
            int num2 = 161803398 - num;
            SeedArray[55] = num2;
            int num3 = 1;
            for (int i = 1; i < 55; i++)
            { 
                int num4 = 21 * i % 55;
                SeedArray[num4] = num3;
                num3 = num2 - num3;
                if (num3 < 0)
                {
                    num3 += int.MaxValue;
                }
                num2 = SeedArray[num4];
            }
            for (int j = 1; j < 5; j++)
            {
                for (int k = 1; k < 56; k++)
                {
                    SeedArray[k] -= SeedArray[1 + (k + 30) % 55];
                    if (SeedArray[k] < 0)
                    {
                        SeedArray[k] += int.MaxValue;
                    }
                }
            }
            inext = 0;
            inextp = 21;
            Seed = 1;
        }
        public double InternalSample()
        {
            int retVal;
            int locINext = inext = 0;
            int locINextp = inextp = 21;

            if (++locINext >= 56) locINext = 1;
            if (++locINextp >= 56) locINextp = 1;

            retVal = SeedArray[locINext] - SeedArray[locINextp];

            if (retVal == MBIG) retVal--;
            if (retVal < 0) retVal += MBIG;

            SeedArray[locINext] = retVal;


            inext = locINext;
            inextp = locINextp;

            return (double)retVal;
        }
    }
}
