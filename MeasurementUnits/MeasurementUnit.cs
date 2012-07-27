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


        public MeasurementUnit()
        {

        }
        public MeasurementUnit(double quantity, Unit unit) : base()
        {
            this.Unit = unit;
            //int powerOfTen = PowerOfTen(quantity);
            //if (powerOfTen != 0)
            //{
            //    Prefix newPrefix = FindClosestPrefix((int)unit.Prefix + powerOfTen);
            //    int difference = (int)unit.Prefix + powerOfTen - (int)newPrefix;
            //    Quantity = quantity / Math.Pow(10, powerOfTen - difference);
            //    unit.Prefix = newPrefix;
            //}
            //else
            //{
                this.Quantity = quantity;   
            //}
        }
            

        #region Unit & Unit Operators

        public static MeasurementUnit operator +(MeasurementUnit u1, MeasurementUnit u2)
        {
            var newUnit = new MeasurementUnit();
            newUnit.Unit = u1.Unit + u2.Unit;
            newUnit.Quantity = u1.Quantity * Math.Pow(10, u1.Unit.DeterminePower10(newUnit.Unit.Prefix)) + u2.Quantity * Math.Pow(10, u2.Unit.DeterminePower10(newUnit.Unit.Prefix));
            return newUnit;
        }

        public static MeasurementUnit operator -(MeasurementUnit u1, MeasurementUnit u2)
        {
            u2 *= -1;
            return u1 + u2;
        }

        public static MeasurementUnit operator *(MeasurementUnit u1, MeasurementUnit u2)
        {
            var unit = new MeasurementUnit();
            unit.Unit = u1.Unit * u2.Unit;
            unit.Quantity = u1.Quantity * u2.Quantity * Math.Pow(10, unit.Unit.Power10);
            unit.Unit.Power10 = 0;
            return unit;
        }

        public static MeasurementUnit operator /(MeasurementUnit u1, MeasurementUnit u2)
        {
            return u1 * (1 / u2);
        }
        #endregion

        #region Double & Unit Operators

        public static MeasurementUnit operator +(double number, MeasurementUnit unit)
        {
            var newUnit = new MeasurementUnit();
            newUnit.Unit = unit.Unit;
            newUnit.Quantity = unit.Quantity + number;
            return newUnit;
        }

        public static MeasurementUnit operator +(MeasurementUnit unit, double number)
        {
            return number + unit;
        }

        public static MeasurementUnit operator -(double number, MeasurementUnit unit)
        {
            var newUnit = new MeasurementUnit();
            newUnit.Unit = unit.Unit;
            newUnit.Quantity = number - unit.Quantity;
            return newUnit;
        }

        public static MeasurementUnit operator -(MeasurementUnit unit, double number)
        {
            var newUnit = new MeasurementUnit();
            newUnit.Unit = unit.Unit;
            newUnit.Quantity = unit.Quantity - number;
            return newUnit;
        }

        public static MeasurementUnit operator *(double number, MeasurementUnit unit)
        {
            var newUnit = new MeasurementUnit();
            newUnit.Unit = unit.Unit;
            newUnit.Quantity = unit.Quantity * number;
            return newUnit;
        }

        public static MeasurementUnit operator *(MeasurementUnit unit, double number)
        {
            return number * unit;
        }

        public static MeasurementUnit operator /(double number, MeasurementUnit unit)
        {
            var newUnit = new MeasurementUnit();
            newUnit.Unit = unit.Unit.Pow(-1);
            newUnit.Quantity = number / unit.Quantity;
            return newUnit;
        }

        public static MeasurementUnit operator /(MeasurementUnit unit, double number)
        {
            var newUnit = new MeasurementUnit();
            newUnit.Unit = unit.Unit;
            newUnit.Quantity = unit.Quantity / number;
            return unit;
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

    }
}
