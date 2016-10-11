using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Metric
{
    class UnitParser
    {
        internal static Unit Parse(string s)
        {
            s = s.Replace(" ", "");
            var digits = new StringBuilder();
            foreach (var x in s)
            {
                if(char.IsDigit(x) || char.IsPunctuation(x))
                {
                    digits.Append(x);
                }
                else
                {
                    break;
                }
            }
            double quantity = double.Parse(digits.ToString());
            s = s.Substring(digits.Length, s.Length - digits.Length);
            s = ConvertSuperscript(s);
            var rational = s.Split('/');
            Unit numerator = Polynome(rational[0], true);
            if (rational.Length == 2)
            {
                Unit denominator = Polynome(rational[1], false);
                return quantity * numerator * denominator;
            }
            return quantity * numerator;
        }

        static Unit Polynome(string s, bool numerator)
        {
            char dot;
            char.TryParse(SingleUnit.Dot, out dot);
            var sUnits = s.Split('*', dot);
            var units = new Stack<Unit>();
            foreach (string singleUnit in sUnits)
            {
                int pw = 1;
                var unit = singleUnit.Split('^');
                if (unit.Length > 1)
                {
                    pw = int.Parse(unit[1]);
                }
                if (!numerator)
                {
                    pw *= -1;
                }
                Unit u = LinearUnit(unit[0]);
                units.Push(u.Pow(pw));
            }
            if (units.Count == 1)
            {
                return units.Pop();
            }
            else
            {
                Unit result = units.Pop();
                while(units.Count > 0) 
                {
                    result *= units.Pop();
                }
                return result;
            }

        }

        static Unit LinearUnit(string linearUnit)
        {
            string test = linearUnit;
            for (int i = linearUnit.Length - 1; i >= 0; i--)
            {
                test = linearUnit.Substring(i);
                if(Unit.Exists(test))
                {
                    if (linearUnit.Length - test.Length == 1)
                    {
                        var px = (Prefix)Enum.Parse(typeof(Prefix), linearUnit[0].ToString());
                        return Unit.Create(px, test);
                    }
                    else return Unit.Create(test);
                }
            }
            throw new FormatException($"Unknown unit: '{linearUnit}'");
        }
        static string ConvertSuperscript(string value)
        {
            string normalized = Normalize(value);
            var stringBuilder = new StringBuilder();
            bool added = false;
            foreach (char character in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(character);
                if (category != UnicodeCategory.NonSpacingMark)
                {
                    if (character == '^') added = true;
                    if ((category == UnicodeCategory.DecimalDigitNumber || character == '-') && !added)
                    {
                        stringBuilder.Append('^');
                        added = true;
                    }
                    stringBuilder.Append(character);
                }
            }

            return stringBuilder.ToString();
        }
        static string Normalize(string s)
        {
            s = s.Replace(SingleUnit.Minus, "-");
            s = s.Replace(SingleUnit.Dot, "*");
            for (int i = 0; i < 10; i++)
            {
                s = s.Replace(SingleUnit.SS(i), i.ToString());
            }
            return s;
        }

    }
}
