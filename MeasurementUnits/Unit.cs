using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasurementUnits
{
    public enum BaseUnit { m = 2, g = 3, s = 5, A = 7, K = 11, cd = 13, mol = 17 }

    public class Unit : IEnumerable<Unit>
    {
        #region Fields & Properties
        internal int Power10 { get; set; }
        internal event PropertyChangedEventHandler PropertyChanged;
        protected int _power;
        protected Prefix _prefix;

        public virtual Prefix Prefix { get { return _prefix; } set { if (PropertyChanged != null) PropertyChanged((value - _prefix) * _power, new PropertyChangedEventArgs("Prefix")); _prefix = value; } }
        public virtual int Power { get { return _power; } set { _power = value; } }
        public BaseUnit BaseUnit { get; set; }
        #endregion
        #region Constructors
        public Unit()
        {
            _power = 1;
            Power10 = 0;
        }
        public Unit(BaseUnit baseUnit) : this()
        {
            this.BaseUnit = baseUnit;
        }
        public Unit(BaseUnit baseUnit, int power) : this(baseUnit)
        {
            this.Power = power;
        }
        public Unit(Prefix prefix, BaseUnit baseUnit) : this(baseUnit)
        {
            this.Prefix = prefix;
        }
        public Unit(Prefix prefix, BaseUnit baseUnit, int power) : this(baseUnit, power)
        {
            this.Prefix = prefix;
        }
        #endregion
        #region Operators
        public static Unit operator +(Unit u1, Unit u2)
        {
            if (u1.IsAddable(u2))
            {
                var newUnit = new Unit();
                newUnit.Prefix = AveragePrefix(u1.Prefix, u2.Prefix);  
                newUnit.BaseUnit = u1.BaseUnit;
                newUnit.Power = u1.Power;
                return newUnit;
            }
            else
            {
                throw new GrandmothersAndFrogsException("You can't mix them. You just can't");
            }
        }
        public static Unit operator -(Unit u1, Unit u2)
        {
            return u1 + u2;
        }
        public static Unit operator *(Unit u1, Unit u2)
        {
            if (u1.BaseUnit == u2.BaseUnit && u1.BaseUnit != 0)
            {
                var newUnit = new Unit();

                var averagePrefix = AveragePrefix(u1.Prefix, u2.Prefix);
                newUnit.Power10 = u1.DeterminePower10(averagePrefix) + u2.DeterminePower10(averagePrefix);

                newUnit.Prefix = averagePrefix;
                newUnit.BaseUnit = u1.BaseUnit;
                newUnit.Power = u1.Power + u2.Power;
                return newUnit;
            }
            else
            {
                return ComplexUnit.Multiply(u1, u2);
            }
        }
        public static Unit operator /(Unit u1, Unit u2)
        {
            return u1 * u2.Pow(-1);
        }
        #endregion
        #region Methods
        public virtual bool IsAddable(Unit u)
        {
            if (Power == u.Power && BaseUnit == u.BaseUnit && u is Unit)
            {
                return true;
            }
            return false;
        }
        public virtual Unit HasFactor(Unit unit, ref int factor)
        {
            if (unit.BaseUnit == this.BaseUnit)
            {
                factor = this.Power / unit.Power;
            }
            else factor = 0;
            return this;
        }
        public override string ToString()
        {
            if (Power == 0)
            {
                return "";
            }
            else if (Power == 1)
            {
                return Prefix != 0 ? string.Format("{0}{1}", Prefix, BaseUnit) : string.Format("{0}", BaseUnit);
            }
            else
            {
                return Prefix != 0 ? string.Format("{0}{1}{2}", Prefix, BaseUnit, Str.SS(Power)) : string.Format("{0}{1}", BaseUnit, Str.SS(Power));
            }
        }
        public static Prefix AveragePrefix(params Prefix[] prefixes)
        {
            var average = prefixes.Average(x => (int)x);
            var averagePrefix = average != 0 ? Enum.GetValues(typeof(Prefix)).Cast<Prefix>().First(x => (int)x >= average) : 0;
            return averagePrefix;
        }
        public static Prefix FindClosestPrefix(int powerOfTen)
        {
            int absolutePower = Math.Abs(powerOfTen);
            Prefix prefix;
            if (absolutePower < 25)
            {
                if (absolutePower % 3 == 0 || absolutePower < 3)
                {
                    prefix = (Prefix)powerOfTen;
                }
                else
                {
                    prefix = Enum.GetValues(typeof(Prefix)).Cast<Prefix>().Where(x => (int)x <= powerOfTen).Max();
                }
            }
            else
            {
                prefix = Math.Sign(powerOfTen) == 1 ? Prefix.Y : Prefix.y;  
            }
            return prefix;
        }
        public virtual int DeterminePower10(Prefix prefix)
        {
            return Power * ((int)Prefix - (int)prefix);
        }
        public virtual Unit Pow(int power)
        {
            var powered = new Unit();
            powered.Prefix = this.Prefix;
            powered.BaseUnit = this.BaseUnit;
            powered.Power = this.Power * power;
            return powered;
        }
        public virtual IEnumerator<Unit> GetEnumerator()
        {
            yield return this;
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
