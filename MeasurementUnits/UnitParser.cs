using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasurementUnits
{
    internal class UnitParser
    {
        internal static Unit Parse(string s)
        {
            s = s.Replace(" ", "");
            s = ConvertSuperscript(s);
            var rational = s.Split('/');
            Unit numerator = Polynome(rational[0], true);
            if (rational.Length == 2)
            {
                Unit denominator = Polynome(rational[1], false);
                return Unit.Multiply(numerator, denominator);
            }
            return numerator;
        }

        private static Unit Polynome(string s, bool numerator)
        {
            char dot;
            char.TryParse(Stringifier.Dot, out dot);
            var sUnits = s.Split('*', dot);
            var units = new List<Unit>();
            foreach (string singleUnit in sUnits)
            {
                int pw = 1;
                var unit = singleUnit.Split('^');
                if (unit.Count() > 1)
                {
                    pw = int.Parse(unit[1]);
                }
                if (!numerator)
                {
                    pw *= -1;
                }
                Unit u = LinearUnit(unit[0]);
                units.Add(u.Pow(pw));
            }
            return units.Count == 1 ? units[0] : Unit.Multiply(units.ToArray());
        }

        private static Unit LinearUnit(string linearUnit)
        {
            Unit u = null;
            string test = linearUnit;
            for (int i = linearUnit.Length - 1; i >= 0; i--)
            {
                test = linearUnit.Substring(i);
                try
                {
                    u = Unit.GetBySymbol(test);
                    break;
                }
                catch(InvalidOperationException) { }
            }
            if (u == null) throw new FormatException("What is this '" + linearUnit + "' you are referring to? I have never heard of it.");
            if (linearUnit.Length - test.Length == 1)
            {
                Prefix px = (Prefix)Enum.Parse(typeof(Prefix), linearUnit[0].ToString());
                u = u.ChangePrefix(px);
            }
            return u;
        }
        internal static string ConvertSuperscript(string value)
        {
            string normalized = Normalize(value);
            StringBuilder stringBuilder = new StringBuilder();
            bool added = false;
            foreach (char character in normalized)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    if (character == '^') added = true;
                    if ((unicodeCategory == UnicodeCategory.DecimalDigitNumber || character == '-') && !added)
                    {
                        stringBuilder.Append('^');
                        added = true;
                    }
                    stringBuilder.Append(character);
                }
            }

            return stringBuilder.ToString();
        }
        private static string Normalize(string s)
        {
            s = s.Replace(Stringifier.Minus, "-");
            s = s.Replace(Stringifier.Dot, "*");
            for (int i = 0; i < 10; i++)
            {
                s = s.Replace(Stringifier.SS(i), i.ToString());
            }
            return s;
        }

    }
}
