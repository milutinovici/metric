using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasurementUnits
{
    public class UnitParser
    {
        public static Unit Parse(string s)
        {
            
            List<Unit> units = new List<Unit>();

            var sUnits = s.Split('*');
            foreach (string sUnit in sUnits)
            {
                int pw = 1;
                BaseUnit bu = 0;
                Prefix px = 0;
                var unit = sUnit.Split('^');
                if (unit.Count() > 1)
                {
                    pw = int.Parse(unit[1]);
                }
                if (unit[0].Length > 1)
                {
                    px = (Prefix)Enum.Parse(typeof(Prefix), unit[0][0].ToString());
                    unit[0] = unit[0].Substring(1, unit[0].Length - 1);
                }
                Unit u;
                try
                {
                    bu = (BaseUnit)Enum.Parse(typeof(BaseUnit), unit[0]);
                    u = new Unit(px, bu, pw);
                }
                catch (ArgumentException)
                {
                    u = ComplexUnit.GetBySymbol(unit[0]);
                    u.Prefix = px;
                    u.Power = pw;
                }
                units.Add(u);
            }
            if (units.Count == 1)
            {
                return units[0];
            }
            else
            {
                return new ComplexUnit(units.ToArray());
            }
        }
    }
}
