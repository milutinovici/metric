using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasurementUnits
{
    public class MeasurementUnit
    {
        public double Quantity { get; set; }
        internal Unit Unit { get; set; }

        public MeasurementUnit(double quantity, Unit unit) : base()
        {
            this.Unit = unit;
            this.Quantity = quantity;   
        }

        #region Unit & Unit Operators
        public static MeasurementUnit operator +(MeasurementUnit u1, MeasurementUnit u2)
        {
            var unit = u1.Unit + u2.Unit;
            var quantity  = u1.Quantity * Math.Pow(10, u1.Unit.DeterminePower10(unit.Prefix)) + u2.Quantity * Math.Pow(10, u2.Unit.DeterminePower10(unit.Prefix));
            return new MeasurementUnit(quantity, unit);
        }
        public static MeasurementUnit operator -(MeasurementUnit u1, MeasurementUnit u2)
        {
            u2 *= -1;
            return u1 + u2;
        }
        public static MeasurementUnit operator *(MeasurementUnit u1, MeasurementUnit u2)
        {
            var unit =  u1.Unit * u2.Unit;
            var quantity = u1.Quantity * u2.Quantity * Math.Pow(10, unit.Power10);
            unit.Power10 = 0;
            return new MeasurementUnit(quantity, unit);
        }
        public static MeasurementUnit operator /(MeasurementUnit u1, MeasurementUnit u2)
        {
            return u1 * (1 / u2);
        }
        #endregion

        #region Double & Unit Operators
        public static MeasurementUnit operator +(double number, MeasurementUnit unit)
        {
            return new MeasurementUnit(unit.Quantity + number, unit.Unit);
        }
        public static MeasurementUnit operator +(MeasurementUnit unit, double number)
        {
            return number + unit;
        }
        public static MeasurementUnit operator -(double number, MeasurementUnit unit)
        {
            return new MeasurementUnit(number - unit.Quantity, unit.Unit);
        }
        public static MeasurementUnit operator -(MeasurementUnit unit, double number)
        {
            return new MeasurementUnit(unit.Quantity - number, unit.Unit);
        }
        public static MeasurementUnit operator *(double number, MeasurementUnit unit)
        {
            return new MeasurementUnit(unit.Quantity * number, unit.Unit);
        }
        public static MeasurementUnit operator *(MeasurementUnit unit, double number)
        {
            return number * unit;
        }
        public static MeasurementUnit operator /(double number, MeasurementUnit unit)
        {
            return new MeasurementUnit(number / unit.Quantity, unit.Unit.Pow(-1));
        }
        public static MeasurementUnit operator /(MeasurementUnit unit, double number)
        {
            return new MeasurementUnit(unit.Quantity / number, unit.Unit);
        }
        #endregion

        private static int PowerOfTen(double quantity)
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
                string test = Convert.ToString(quantity);
                int s = (test.IndexOf(".") + 1);
                powerOfTen = -((test.Length) - s);
            }
            return powerOfTen;
        }
        public override string ToString()
        {
            if (Unit is ComplexUnit)
            {
                ((ComplexUnit)Unit).FindDerivedUnits();
                Quantity *= Math.Pow(10, Unit.Power10);
                Unit.Power10 = 0;
            }
            return string.Format("{0} {1}", Quantity, Unit);
        }

        public override bool Equals(object obj)
        {
            var unit = obj as MeasurementUnit;
            if (unit != null)
            {
                if (Unit.IsAddable(unit.Unit))
                {
                    var power1 = Unit.Aggregate(0, (x, y) => (int)y.Prefix + y.Power10);
                    var power2 = unit.Unit.Aggregate(0, (x, y) => (int)y.Prefix + y.Power10);
                    return Math.Pow(10, power1) * Quantity == Math.Pow(10, power2) * unit.Quantity;
                }
                return false;
            }
            return false;
        }
    }
}
