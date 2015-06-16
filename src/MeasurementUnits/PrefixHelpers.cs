using System;
using System.Linq;

namespace MeasurementUnits
{
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
