using System;
using System.Collections.Generic;

namespace MeasurementUnits
{
    public enum BaseUnit : ulong { m = 2, g = 3, s = 5, A = 7, K = 11, cd = 13, mol = 17 }

    public static class BaseHelpers
    {
        public static BaseUnit GreatestCommonDenominator(BaseUnit a, BaseUnit b)
        {
            return b == 0 ? a : GreatestCommonDenominator(b, (BaseUnit)((ulong)a % (ulong)b));
        }
        public static BaseUnit Divide(BaseUnit a, BaseUnit b)
        {
            return (BaseUnit)((ulong)a / (ulong)b);
        }
        
        public static IEnumerable<int> PrimeFactors(this BaseUnit unit)
        {
            var num = (ulong)unit;
            var bases = Enum.GetValues(typeof(BaseUnit));
            foreach (int bas in bases)
            {
                if(num == 1)
                {
                    break;
                }
                if(num % (ulong)bas == 0)
                {
                    num /= (ulong)bas;
                    yield return bas;
                }
            }
        }
        
        public static int HasFactor(this BaseUnit unit, BaseUnit factor)
        {
            if (factor == 0)
            {
                return 0;
            }
            int power = 0;
            var u = (ulong)unit;
            var f = (ulong)factor;
            while (u > 1 && u % f == 0)
            {
                u = u / f;
                power++;
            }
            return power;
        }

        public static BaseUnit AverageFactors(this BaseUnit a, BaseUnit b)
        {
            ulong average = 1;
            var bases = Enum.GetValues(typeof(BaseUnit));
            foreach (BaseUnit bas in bases)
            {
                var factorA = HasFactor(a, bas);
                var factorB = HasFactor(b, bas);

                average *= (ulong)bas.Pow((uint)Math.Abs(factorA - factorB));
            }
            return (BaseUnit)average;
        }
        public static BaseUnit Pow(this BaseUnit u, uint pow)
        {
            ulong ret = 1;
            ulong x = (ulong)u;
            while (pow != 0)
            {
                if ((pow & 1) == 1)
                    ret *= x;
                x *= x;
                pow >>= 1;
            }
            return (BaseUnit)ret;
        }
        public static Tuple<BaseUnit, BaseUnit> Reduce(BaseUnit a, BaseUnit b)
        {
            var gcd = BaseHelpers.GreatestCommonDenominator(a, b);

            if(a != 0 && b != 0 && gcd != 0)
            {
                return Tuple.Create(BaseHelpers.Divide(a , gcd), BaseHelpers.Divide(b , gcd));
            }
            else 
            {
                return Tuple.Create(a,b);
            }
        }
    }
}