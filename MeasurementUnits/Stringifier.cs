using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasurementUnits
{
    public class Stringifier
    {
        private static readonly string[] SuperscriptDigits = new[] { "\u2070", "\u00b9", "\u00b2", "\u00b3", "\u2074", "\u2075", "\u2076", "\u2077", "\u2078", "\u2079" };
        public static readonly string Dot = "\u00B7";
        public static readonly string Minus = "\u207B";

        internal static string UnitToString(Prefix prefix, string unit, int power, bool fancy = true)
        {
            var s = new StringBuilder();
            if (power == 0)
                return "";
            if (prefix != 0)
                s.Append(prefix);
            s.Append(unit);
            if (power != 1)
            {
                if (fancy)
                    s.Append(SS(power));
                else
                    s.Append("^").Append(power);
            }
            return s.ToString();
        }

        internal static string MultipleUnitsToString(IEnumerable<Unit> units, bool fancy, bool useDivisor)
        {
            var name = new StringBuilder();
            var multiplier = fancy ? Dot : "*";
            var f = fancy ? "" : "c"; 
            f += "i";
            var group = units.GroupBy(x => Math.Sign(x.Power)).ToArray();
            foreach (var unit in group.ElementAt(0))
            {
                name.Append(unit.ToString(f)).Append(multiplier);
            }
            if (group.Count() > 1)
            {
                if (useDivisor)
                {
                    foreach (var unit in group.ElementAt(1))
                    {
                        name.Append(unit.ToString(f)).Append(multiplier);
                    }
                }
                else
                {
                    name.Remove(name.Length - 1, 1);
                    name.Append("/");
                    foreach (var unit in group.ElementAt(1))
                    {
                        name.Append(unit.Pow(-1).ToString(f)).Append(multiplier);
                    }
                }
            }
            return name.Remove(name.Length - 1, 1).ToString();
        }

        public static string SS(int power)
        {
            var sb = new StringBuilder();
            if (power < 0)
            {
                sb.Append(Minus);
                power *= -1; 
            }
            var ints = power.ToString().ToCharArray().Select(x => (int)char.GetNumericValue(x));
            
            foreach (var num in ints)
            {
                sb.Append(SuperscriptDigits[num]);
            }
            return sb.ToString();
        }

    }
}
