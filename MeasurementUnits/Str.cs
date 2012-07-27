using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MeasurementUnits
{
    public class Str
    {
        public List<string> Numerator { get; set; }
        public List<string> Denominator { get; set; } 

        private static readonly string[] SuperscriptDigits = new string[] { "\u2070", "\u00b9", "\u00b2", "\u00b3", "\u2074", "\u2075", "\u2076", "\u2077", "\u2078", "\u2079" };
        public static readonly string dot = "\u00B7";
        public static readonly string minus = "\u207B";

        public Str()
        {
            Numerator = new List<string>();
            Denominator = new List<string>();
        }

        public string GetStringRepresentation()
        {
            if (Numerator.Count > 0 && Denominator.Count > 0)
            {
                return string.Format("{0}/{1}", AppendStrings(Numerator), AppendStrings(Denominator));
            }
            else if (Numerator.Count > 0)
            {
                return string.Format("{0}", AppendStrings(Numerator));
            }
            else if (Denominator.Count > 0)
            {
                return string.Format("1/{0}", AppendStrings(Denominator));
            }
            else
            {
                return "";
            }
        }

        internal static string AppendStrings(IEnumerable<string> units)
        {
            StringBuilder s = new StringBuilder();
            foreach (var unit in units)
            {
                s.Append(dot);
                s.Append(unit);
            }
            return s.Remove(0, 1).ToString();
        }

        internal static string UnitToString(Prefix prefix, string unit, int power)
        {
            StringBuilder s = new StringBuilder();
            if (prefix != 0)
                s.Append(prefix);
            s.Append(unit);
            if (power != 1)
                s.Append(SS(power));
            return s.ToString();
        }

        public static string SS(int power)
        {
            StringBuilder sb = new StringBuilder();
            if (power < 0)
            {
                sb.Append(minus);
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
