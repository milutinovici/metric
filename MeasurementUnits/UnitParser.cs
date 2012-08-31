using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasurementUnits
{
    public class UnitParser
    {
        public static Unit Parse(string s)
        {
            s = s.Replace(" ", "");
            //s = ConvertSuperscript(s);
            var rational = s.Split('/');
            Unit numerator = Polynome(rational[0], true);
            if (rational.Length == 2)
            {
                Unit denominator = Polynome(rational[1], false);
                return ComplexUnit.Multiply(numerator, denominator);
            }
            return numerator;
        }

        private static Unit Polynome(string s, bool numerator)
        {
            char dot;
            char.TryParse(Str.dot, out dot);
            var sUnits = s.Split('*', dot);
            List<Unit> units = new List<Unit>();
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
                u.Power = pw;
                units.Add(u);
            }
            if (units.Count == 1)
            {
                return units[0];
            }
            else
            {
                return ComplexUnit.Multiply(units.ToArray());
            }
        }

        private static Unit LinearUnit(string linearUnit)
        {
            Unit u = null;
            BaseUnit bu;
            bool success = false;
            string test = linearUnit;
            for (int i = linearUnit.Length - 1; i >= 0; i--)
            {
                test = linearUnit.Substring(i);
                success = Enum.TryParse<BaseUnit>(test, out bu);
                if (success)
                {
                    u = new Unit(bu);
                    break;
                }
                try
                {
                    u = ComplexUnit.GetBySymbol(test);
                    break;
                }
                catch { }
            }
            if (u == null) throw new FormatException("What is this '" + linearUnit + "' you are referring to? I have never heard of it.");
            if (linearUnit.Length - test.Length == 1)
            {
                Prefix px = (Prefix)Enum.Parse(typeof(Prefix), linearUnit[0].ToString());
                u.Prefix = px;
            }
            return u;
        }
        //public static string ConvertSuperscript(string value)
        //{
        //    string stringFormKd = value.Normalize(NormalizationForm.FormKD);
        //    stringFormKd = stringFormKd.Replace((char)8722, '-');
        //    StringBuilder stringBuilder = new StringBuilder();
        //    bool added = false;
        //    foreach (char character in stringFormKd)
        //    {
        //        UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
        //        if (unicodeCategory != UnicodeCategory.NonSpacingMark)
        //        {
        //            if (character == '^') added = true;
        //            if ((unicodeCategory == UnicodeCategory.DecimalDigitNumber || character == '-') && !added)
        //            {
        //                stringBuilder.Append('^');
        //                added = true;
        //            }
        //            stringBuilder.Append(character);
        //        }
        //    }

        //    return stringBuilder.ToString().Normalize(NormalizationForm.FormKC);
        //}
    }
}
