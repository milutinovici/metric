using System.Linq;
using System.Text;

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
