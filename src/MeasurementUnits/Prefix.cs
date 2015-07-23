using System;
using System.Linq;

namespace MeasurementUnits
{
    public enum Prefix : sbyte
    {
        d = -1, c = -2, m = -3, μ = -6, n = -9, p = -12, f = -15, a = -18, z = -21, y = -24,
        da = 1, h = 2, k = 3, M = 6, G = 9, T = 12, P = 15, E = 18, Z = 21, Y = 24
    }

    internal static class PrefixHelpers
    {
        internal static Prefix AveragePrefix(params Prefix[] prefixes)
        {
            var average = prefixes.Average(x => (int)x);
            var averagePrefix = average != 0 ? Enum.GetValues(typeof(Prefix)).Cast<Prefix>().First(x => (int)x >= average) : 0;
            return averagePrefix;
        }

        internal static Prefix FindClosestPrefix(int powerOfTen)
        {
            int absolutePower = Math.Abs(powerOfTen);
            Prefix prefix;
            if (absolutePower < 25)
            {
                if (absolutePower % 3 == 0 || absolutePower < 3)
                {
                    prefix = (Prefix)powerOfTen;
                }
                else
                {
                    prefix = Enum.GetValues(typeof(Prefix)).Cast<Prefix>().Where(x => (int)x <= powerOfTen).Max();
                }
            }
            else
            {
                prefix = Math.Sign(powerOfTen) == 1 ? Prefix.Y : Prefix.y;
            }
            return prefix;
        }

        internal static int Power10(double quantity)
        {
            int powerOfTen = 0;
            if (Math.Abs(quantity) > 1)
            {
                while (quantity % 10 == 0)
                {
                    powerOfTen++;
                    quantity /= 10;
                }
            }
            else
            {
                while (quantity % 1 != 0)
                {
                    quantity *= 10;
                    powerOfTen--;
                }
            }
            return powerOfTen;
        }
    }
}
