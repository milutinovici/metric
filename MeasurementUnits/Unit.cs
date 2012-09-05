using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MeasurementUnits
{
    public enum BaseUnit { m = 2, g = 3, s = 5, A = 7, K = 11, cd = 13, mol = 17 }

    public sealed class Unit : IEnumerable<Unit>
    {
        #region Fields & Properties
        private BaseUnit BaseUnit { get; set; } 
        public int Power { get; private set; }
        public Prefix Prefix { get; private set; }
        public double Quantity { get; private set; }
        public string UnitName { get; private set; }
        public IList<Unit> Units { get; private set; }
        #region Derived Units
        private static readonly IEnumerable<Unit> DerivedUnits = new List<Unit> 
        {
            new Unit(unitName :"Ω", units : new[] { new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -3), new Unit(BaseUnit.A, -2)}),
            new Unit(unitName :"V", units : new[] { new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -3), new Unit(BaseUnit.A, -1)}),
            new Unit(unitName :"H", units : new[] { new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s,-2), new Unit(BaseUnit.A, -2)}),
            new Unit(unitName :"Wb", units : new[] { new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -2), new Unit(BaseUnit.A, -1)}),

            new Unit(unitName :"F", units : new[] { new Unit(Prefix.k, BaseUnit.g, -1), new Unit(BaseUnit.m, -2), new Unit(BaseUnit.s, 4), new Unit(BaseUnit.A, 2)}),
            new Unit(unitName :"S", units : new[] { new Unit(Prefix.k, BaseUnit.g, -1), new Unit(BaseUnit.m, -2), new Unit(BaseUnit.s, 3), new Unit(BaseUnit.A, 2)}),

            new Unit(unitName :"W", units : new[] { new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -3)}),
            new Unit(unitName :"J", units : new[] { new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -2)}),
            new Unit(unitName :"N", units : new[] { new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m), new Unit(BaseUnit.s, -2)}),
            
            new Unit(unitName :"Pa", units : new[] { new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, -1), new Unit(BaseUnit.s, -2)}),
            new Unit(unitName :"T", units : new[] { new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.s, -2), new Unit(BaseUnit.A, -1)}),
            
            new Unit(unitName : "C", units : new[] { new Unit(BaseUnit.s), new Unit(BaseUnit.A)}),
            new Unit(unitName :"Gy", units : new[] { new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -2)}),
            new Unit(unitName :"lx",units : new[] { new Unit(BaseUnit.m, -2), new Unit(BaseUnit.cd)}),
            new Unit(unitName :"kat",units : new[] { new Unit(BaseUnit.s, -1), new Unit(BaseUnit.mol)}),
        };
        #endregion
        #endregion
        #region Constructors
        private Unit()
        {
            this.Units = new ReadOnlyCollection<Unit>(new List<Unit>());
            this.Quantity = 1;
            this.Power = 1;
        }
        public Unit(BaseUnit baseUnit, int power = 1)
            : this()
        {
            this.BaseUnit = baseUnit;
            this.UnitName = baseUnit != 0 ? baseUnit.ToString() : "";
            this.Power = power;
        }
        public Unit(Prefix prefix, BaseUnit baseUnit, int power = 1)
            : this(baseUnit, power)
        {
            this.Prefix = prefix;
        }
        public Unit(double quantity, BaseUnit baseUnit, int power = 1)
            : this(baseUnit, power)
        {
            this.Quantity = quantity;
        }
        public Unit(double quantity, Prefix prefix, BaseUnit baseUnit, int power = 1)
            : this(quantity, baseUnit, power)
        {
            this.Prefix = prefix;
        }
        public Unit(params Unit[] units)
            :this()
        {
            this.Quantity *= units.Aggregate(1.0, (x, y) => x * y.Quantity);
            this.Units = OrderUnits(units.Where(x => x.Power != 0).Select(x => x.Pow(1)));
            foreach (var unit in Units)
            {
                unit.Quantity = 1;
            }
        }
        public Unit(double quantity, params Unit[] units)
            : this(units)
        {
            this.Quantity *= quantity;
        }

        private Unit(double quantity = 1, Prefix prefix = 0, string unitName = null, int power = 1, params Unit[] units)
            : this(quantity, units)
        {
            this.UnitName = unitName;
            this.Prefix = prefix;
            this.Power = power;
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
                return Unit.Multiply(u1, u2);
            }
        }
        public static Unit operator /(Unit u1, Unit u2)
        {
            return u1 * u2.Pow(-1);
        }
        #region Double & Unit Operators
        public static Unit operator +(double number, Unit unit)
        {
            return unit.DifferentQuantity(unit.Quantity + number);
        }
        public static Unit operator +(Unit unit, double number)
        {
            return number + unit;
        }
        public static Unit operator -(double number, Unit unit)
        {
            return unit.DifferentQuantity(number - unit.Quantity);
        }
        public static Unit operator -(Unit unit, double number)
        {
            return unit.DifferentQuantity(unit.Quantity - number);
        }
        public static Unit operator *(double number, Unit unit)
        {
            return unit.DifferentQuantity(unit.Quantity * number);
        }
        public static Unit operator *(Unit unit, double number)
        {
            return number * unit;
        }
        public static Unit operator /(double number, Unit unit)
        {
            return unit.Pow(-1).DifferentQuantity(number / unit.Quantity);
        }
        public static Unit operator /(Unit unit, double number)
        {
            return unit.DifferentQuantity(unit.Quantity / number);
        }
        #endregion
        #endregion
        #region Methods
        private static IList<Unit> OrderUnits(IEnumerable<Unit> units)
        {
            return new ReadOnlyCollection<Unit>(units.OrderByDescending(x => Math.Sign(x.Power)).ThenBy(x => x.UnitName).ToArray());
        }

        public static Unit Multiply(params Unit[] units)
        {
            var quantity = units.Aggregate(1.0, (x, y) => x * y.Quantity);
            var groups = units.SelectMany(x => x).GroupBy(x => x.UnitName);
            var multiplied = groups.AsParallel().Select(group => group.Aggregate((x, y) => x * y)).ToArray();
            return new Unit(quantity, multiplied);
        }
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
                var derived = Unit.DerivedUnits.First(x => x.UnitName == symbol).SelectMany(x => x.Pow(1)).ToArray();
                return new Unit(unitName: symbol, units: derived);
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

        public Unit Pow(int power)
        {
            Unit newUnit;
            double quantity = Math.Pow(Quantity, power);
            int newPower = Power * power;
            if (Units.Count == 0)
            {
                newUnit = new Unit(quantity, Prefix, BaseUnit, newPower);
            }
            else
            {
                var units = Units.Select(x => x.Pow(power)).ToArray();
                newUnit = new Unit(quantity, Prefix, UnitName, newPower, units);
            }
            return newUnit;
        }
        public Unit ChangePrefix(Prefix prefix)
        {
            Unit newUnit;
            int difference = (prefix - Prefix) * Power;
            double quantity = Quantity;// * Math.Pow(10, -difference);
            if (Units.Count == 0)
            {
                newUnit = new Unit(quantity, prefix, BaseUnit, Power);
            }
            else
            {
                var units = Units.OrderBy(x => Math.Abs(difference % x.Power)).ThenBy(x => Math.Abs((int)x.Prefix)).ThenBy(x => Math.Abs(x.Power)).Select(x=>x.Pow(1)).ToArray();
                units[0] = units[0].ChangePrefix(Prefix + difference / units[0].Power);
                newUnit = new Unit(quantity, prefix, UnitName, Power, units);
            }
            return newUnit;
        }
        private Unit DifferentQuantity(double quantity)
        {
            Unit newUnit;
            if (Units.Count == 0)
            {
                newUnit = new Unit(quantity, Prefix, BaseUnit, Power);
            }
            else
            {
                var units = Units.Select(x => x.Pow(1)).ToArray();
                newUnit = new Unit(quantity, Prefix, UnitName, Power, units);
            }
            return newUnit;
        }

        private int Power10Difference(Prefix prefix)
        {
            return Power * ((int)Prefix - (int)prefix);
        }

        private void Normalize()
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

        public string ToString(string format)
        {
            format = format.ToLower();
            bool findDerived = string.IsNullOrEmpty(UnitName) && !format.Contains("b"); // base units only
            if (findDerived) FindDerivedUnits();
            bool fancy = !format.Contains("c"); // common formmating 
            bool useDivisor = !format.Contains("d"); // use '/'
            string quantity = !format.Contains("i") ? Quantity.ToString() : ""; // ignore quantity
            string unitString = "";
            if (string.IsNullOrEmpty(UnitName) && Units.Count() > 0)
            {
                unitString = Stringifier.MultipleUnitsToString(Units, fancy, useDivisor);
            }
            else if (!string.IsNullOrEmpty(UnitName))
            {
                unitString = Stringifier.UnitToString(Prefix, UnitName, Power, fancy);
            }
            return string.Format("{0}{1}", quantity, unitString);
        }

        public IEnumerator<Unit> GetEnumerator()
        {
            if (Units.Count == 0)
            {
                yield return this;
            }
            else
            {
                foreach (var unit in Units)
                {
                    yield return unit;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsAddable(Unit u)
        {
            if (Power == u.Power) 
            {
                if(UnitName == u.UnitName)
                {
                    return true;
                }
                else
                {
                    var a = Units.SelectMany(x => x).Select(x => new { x.Power, x.UnitName });
                    var b = u.Units.SelectMany(x => x).Select(x => new { x.Power, x.UnitName });
                    return !a.Except(b).Any();
                }
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
            return (int)Quantity * this.SelectMany(x => x).Aggregate(0, (x, y) => x + (int)y.Prefix * (int)y.UnitName.GetHashCode() * y.Power);
        }
        #endregion
        #region DerivedUnits

        public void FindDerivedUnits()
        {
            if (string.IsNullOrEmpty(UnitName))
            {
                var units = new List<Unit>();
                Unit d = null;
                Unit r = this;
                while (r.Units.Any() && r.FindDerivedUnitWithSmallestRemain(ref d, ref r))
                {
                    units.Add(d);
                }
                if (r.Units.Count() != 0 || !string.IsNullOrEmpty(r.UnitName))
                {
                    units.AddRange(r.SelectMany(x => x));
                }
                Quantity = d != null ? d.Quantity : r.Quantity;
                Units = OrderUnits(units);
            }
        }

        private bool FindDerivedUnitWithSmallestRemain(ref Unit derived, ref Unit remain)
        {
            var dict = FindPossibleDerivedUnits();
            if (dict.Any())
            {
                var optimal = dict.OrderBy(x => x.Value.Count()).First();
                derived = optimal.Key;
                remain = optimal.Value;
                return true;
            }
            return false;
        }

        private IDictionary<Unit, Unit> FindPossibleDerivedUnits()
        {
            var dict = new ConcurrentDictionary<Unit, Unit>();
            DerivedUnits.AsParallel().ForAll(derivedUnit =>
            {
                int factor = 0;
                var remain = this.HasFactor(derivedUnit, ref factor);
                if (factor != 0)
                {
                    var pow10 = PrefixHelpers.Power10(remain.Quantity);
                    var prfx = PrefixHelpers.FindClosestPrefix(pow10 / factor);
                    var units = derivedUnit.Units.SelectMany(x => x.Pow(1)).ToArray();
                    var quantity = remain.Quantity / Math.Pow(10, (int)prfx * factor);
                    Unit d = new Unit(quantity, prfx, derivedUnit.UnitName, factor, units);
                    dict.AddOrUpdate(d, remain, (x, y) => y);
                }
            });
            return dict;
        }

        public Unit HasFactor(Unit unit, ref int factor)
        {
            Unit u = new Unit();
            if (unit.UnitName == this.UnitName)
            {
                factor = this.Power / unit.Power;
            }
            else
            {
                u = Factor(unit, ref factor);
                if (factor == 0)
                {
                    u = Factor(unit.Pow(-1), ref factor);
                    factor *= -1;
                }
            }
            return u;
        }

        private Unit Factor(Unit possibleFactor, ref int factor)
        {
            var remain = this;
            bool repeat = false;
            Unit reciprocalFactor = possibleFactor.Pow(-1);
            do
            {
                var quotient = remain * reciprocalFactor;
                var quotients = this.SelectMany(x => x).Concat(quotient.SelectMany(x => x)).GroupBy(x => x.UnitName);
                var factors = this.SelectMany(x => x).Concat(possibleFactor.SelectMany(x => x)).GroupBy(x => x.UnitName);

                var qgroup = quotients.Where(group => group.Count() > 1);
                var fgroup = factors.Where(group => group.Count() > 1);
                var rgroup = quotients.Where(group => group.Count() < 2).SelectMany(x => x).Concat(factors.Where(group => group.Count() < 2).SelectMany(x => x))
                    .GroupBy(x => x.UnitName).Where(group => group.Count() > 1);

                bool qTest = !qgroup.Any(x => x.Aggregate(1, (a, b) => a * b.Power) < 0);
                bool fTest = !fgroup.Any(x => x.Aggregate(1, (a, b) => a * b.Power) < 0);
                bool rTest = !rgroup.Any(x => x.Aggregate(1, (a, b) => a * b.Power) < 0);
                repeat = qTest && fTest && rTest;
                if (repeat)
                {
                    remain = quotient;
                    factor++;
                }
            }
            while (repeat);
            return remain;
        }
        #endregion
    }
}
