using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MeasurementUnits
{
    public enum BaseUnit { m = 2, g = 3, s = 5, A = 7, K = 11, cd = 13, mol = 17 }

    public class Unit : IEnumerable<Unit>
    {
        #region Fields & Properties
        internal event PropertyChangedEventHandler PropertyChanged;
        protected int _power;
        protected Prefix _prefix;
        public double Quantity { get; internal set; }
        public virtual Prefix Prefix
        {
            get { return _prefix; }
            set
            {
                var difference = (value - _prefix) * _power;
                Quantity *= Math.Pow(10, -difference);
                if (PropertyChanged != null)
                {
                    PropertyChanged(difference, new PropertyChangedEventArgs("Prefix"));
                }
                
                _prefix = value;

            }
        }
        public virtual int Power { get { return _power; } set { _power = value; } }
        public BaseUnit BaseUnit { get; private set; }
        #endregion
        #region Constructors
        protected Unit()
        {
            Quantity = 1;
            _power = 1;
        }
        public Unit(BaseUnit baseUnit, int power = 1)
            : this()
        {
            this.BaseUnit = baseUnit;
            this.Power = power;
        }
        public Unit(Prefix prefix, BaseUnit baseUnit, int power = 1)
            : this(baseUnit, power)
        {
            this._prefix = prefix;
        }
        public Unit(double quantity, BaseUnit baseUnit, int power = 1)
            : this(baseUnit, power)
        {
            this.Quantity = quantity;
        }
        public Unit(double quantity, Prefix prefix, BaseUnit baseUnit, int power = 1)
            : this(quantity, baseUnit, power)
        {
            this._prefix = prefix;
        }
        #endregion
        #region Operators
        public static Unit operator +(Unit u1, Unit u2)
        {
            if (u1.IsAddable(u2))
            {
                var averagePrefix = PrefixHelpers.AveragePrefix(u1.Prefix, u2.Prefix);
                var quantity = u1.Quantity * Math.Pow(10, u1.Power10Difference(averagePrefix)) + u2.Quantity * Math.Pow(10, u2.Power10Difference(averagePrefix));
                var newUnit = new Unit(quantity, averagePrefix, u1.BaseUnit, u1.Power); 
                return newUnit;
            }
            else
            {
                throw new GrandmothersAndFrogsException("You can't mix them. You just can't");
            }
        }
        public static Unit operator -(Unit u1, Unit u2)
        {
            if (u1.IsAddable(u2))
            {
                var averagePrefix = PrefixHelpers.AveragePrefix(u1.Prefix, u2.Prefix);
                var quantity = u1.Quantity * Math.Pow(10, u1.Power10Difference(averagePrefix)) - u2.Quantity * Math.Pow(10, u2.Power10Difference(averagePrefix));
                var newUnit = new Unit(quantity, averagePrefix, u1.BaseUnit, u1.Power);
                return newUnit;
            }
            else
            {
                throw new GrandmothersAndFrogsException("You can't mix them. You just can't");
            }
        }
        public static Unit operator *(Unit u1, Unit u2)
        {
            if (u1.BaseUnit == u2.BaseUnit && u1.BaseUnit != 0)
            {
                var averagePrefix = PrefixHelpers.AveragePrefix(u1.Prefix, u2.Prefix);
                var power10 = u1.Power10Difference(averagePrefix) + u2.Power10Difference(averagePrefix);
                var quantity = u1.Quantity * u2.Quantity * Math.Pow(10, power10);
                var newUnit = new Unit(quantity, averagePrefix, u1.BaseUnit, u1.Power + u2.Power);

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
        public static Unit GetBySymbol(string symbol)
        {
            BaseUnit bu;
            bool success = Enum.TryParse<BaseUnit>(symbol, out bu);
            if (success)
            {
                return new Unit(bu);
            }
            else
            {
                var derived = ComplexUnit.DerivedUnits.First(x => x.DerivedUnit == symbol).SelectMany(x => x.Pow(1)).ToArray();
                return new ComplexUnit(symbol, derived);
            }
        }

        public static Unit Parse(string s)
        {
            s = s.Replace(" ", "");
            var d = Regex.Split(s, @"[^0-9\.,]+").Where(c => c != "." && c.Trim() != "").First();
            double q = double.Parse(d);
            s = s.Substring(d.Length, s.Length - d.Length);
            var unit = UnitParser.Parse(s);
            unit.Quantity = q;
            return unit;
        }

        public static bool TryParse(string s, out Unit unit)
        {
            try
            {
                unit = UnitParser.Parse(s);
                return true;
            }
            catch
            {
                unit = null;
                return false;
            }
        }

        public virtual Unit Pow(int power)
        {
            var quantity = Math.Pow(Quantity, power);
            var powered = new Unit(quantity, Prefix, BaseUnit, Power * power);
            return powered;
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

        internal virtual int Power10Difference(Prefix prefix)
        {
            return Power * ((int)Prefix - (int)prefix);
        }

        protected void Normalize()
        {
            var power10 = PrefixHelpers.Power10(Quantity);
            var newPrefix = PrefixHelpers.FindClosestPrefix(power10 / Power);
            if (newPrefix != 0)
            {
                Prefix = newPrefix;
                var pow = -((int)newPrefix * Power);
                Quantity *= Math.Pow(10, pow);
            }
        }

        public override string ToString()
        {
            return ToString("");
        }

        public virtual string ToString(string format)
        {
            format = format.ToLower();
            bool fancy = !format.Contains("c");
            string quantity = !format.Contains("i") ? Quantity.ToString() : "" ;
            return string.Format("{0}{1}", quantity, Stringifier.UnitToString(Prefix, BaseUnit.ToString(), Power, fancy));
        }

        public virtual IEnumerator<Unit> GetEnumerator()
        {
            yield return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual bool IsAddable(Unit u)
        {
            if (Power == u.Power && BaseUnit == u.BaseUnit && u is Unit)
            {
                return true;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            var unit = obj as Unit;
            if (unit != null)
            {
                if (IsAddable(unit))
                {
                    var power1 = this.SelectMany(x => x).Aggregate(0, (x, y) => x + (int)y.Prefix);
                    var power2 = unit.SelectMany(x => x).Aggregate(0, (x, y) => x + (int)y.Prefix);
                    return Math.Pow(10, power1) * Quantity == Math.Pow(10, power2) * unit.Quantity;
                }
                return false;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (int)Quantity * this.SelectMany(x => x).Aggregate(0, (x, y) => x + (int)y.Prefix * (int)y.BaseUnit * y.Power);
        }
        #endregion
    }
}
