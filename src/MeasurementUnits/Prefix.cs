using System;
using System.Linq;

namespace MeasurementUnits
{
    public enum Prefix : sbyte
    {
        d = -1, c = -2, m = -3, μ = -6, n = -9, p = -12, f = -15, a = -18, z = -21, y = -24,
        da = 1, h = 2, k = 3, M = 6, G = 9, T = 12, P = 15, E = 18, Z = 21, Y = 24
    }

    static class PrefixHelpers
    {
        internal static Prefix FindClosestPrefix(int powerOfTen)
        {
            int absolutePower = Math.Abs(powerOfTen);
            Prefix prefix;
            if (absolutePower < 25)
            {
                var mod = absolutePower % 3;
                if (mod % 3 == 0 || absolutePower < 3)
                {
                    prefix = (Prefix)powerOfTen;
                }
                else
                {
                    prefix = mod == 1 ? (Prefix)((absolutePower - 1) * Math.Sign(powerOfTen)) : (Prefix)((absolutePower + 1) * Math.Sign(powerOfTen));
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
                while (Math.Abs(quantity % 10) < double.Epsilon)
                {
                    powerOfTen++;
                    quantity /= 10;
                }
            }
            else
            {
                while (Math.Abs(quantity % 1) > double.Epsilon)
                {
                    quantity *= 10;
                    powerOfTen--;
                }
            }
            return powerOfTen;
        }
    }
}
