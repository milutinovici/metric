using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MeasurementUnits
{
    public class ComplexUnit : Unit
    {
        #region Properties
        public IList<Unit> Units { get; set; }
        public override int Power
        {
            get
            {
                return _power;
            }
            set
            {
                var quotient = value / _power;
                _power = value;
                Units = Units.Select(x => x.Pow(quotient)).ToArray();
            }
        }
        public override Prefix Prefix
        {
            get
            {
                return base.Prefix;
            }
            set
            {
                int pvalue = (value - _prefix) * _power;
                var s = Units.OrderBy(x => Math.Abs(pvalue % x.Power)).ThenBy(x => Math.Abs((int)x.Prefix)).ThenBy(x => Math.Abs(x.Power)).First();
                s.Prefix += pvalue / s.Power;
                base.Prefix = value;
            }
        }
        #region Derived Units
        internal static readonly IEnumerable<ComplexUnit> DerivedUnits = new List<ComplexUnit> 
        {
            new ComplexUnit(unitName :"Ω", units : new[] { new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -3), new Unit(BaseUnit.A, -2)}),
            new ComplexUnit(unitName :"V", units : new[] { new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -3), new Unit(BaseUnit.A, -1)}),
            new ComplexUnit(unitName :"H", units : new[] { new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s,-2), new Unit(BaseUnit.A, -2)}),
            new ComplexUnit(unitName :"Wb", units : new[] { new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -2), new Unit(BaseUnit.A, -1)}),

            new ComplexUnit(unitName :"F", units : new[] { new Unit(Prefix.k, BaseUnit.g, -1), new Unit(BaseUnit.m, -2), new Unit(BaseUnit.s, 4), new Unit(BaseUnit.A, 2)}),
            new ComplexUnit(unitName :"S", units : new[] { new Unit(Prefix.k, BaseUnit.g, -1), new Unit(BaseUnit.m, -2), new Unit(BaseUnit.s, 3), new Unit(BaseUnit.A, 2)}),

            new ComplexUnit(unitName :"W", units : new[] { new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -3)}),
            new ComplexUnit(unitName :"J", units : new[] { new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -2)}),
            new ComplexUnit(unitName :"N", units : new[] { new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m), new Unit(BaseUnit.s, -2)}),
            
            new ComplexUnit(unitName :"Pa", units : new[] { new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, -1), new Unit(BaseUnit.s, -2)}),
            new ComplexUnit(unitName :"T", units : new[] { new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.s, -2), new Unit(BaseUnit.A, -1)}),
            
            new ComplexUnit(unitName : "C", units : new[] { new Unit(BaseUnit.s), new Unit(BaseUnit.A)}),
            new ComplexUnit(unitName :"Gy", units : new[] { new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -2)}),
            new ComplexUnit(unitName :"lx",units : new[] { new Unit(BaseUnit.m, -2), new Unit(BaseUnit.cd)}),
            new ComplexUnit(unitName :"kat",units : new[] { new Unit(BaseUnit.s, -1), new Unit(BaseUnit.mol)}),
        };
        #endregion
        #endregion
        #region Constructors

        public ComplexUnit(params Unit[] units)
        {
            Quantity *= units.Aggregate(1.0, (x, y) => x * y.Quantity);
            Units = OrderUnits(units.Where(x => x.Power != 0));
            foreach (var unit in Units)
            {
                unit.PropertyChanged += unit_PropertyChanged;
                unit.Quantity = 1;
            }
        }
        public ComplexUnit(double quantity, params Unit[] units)
            : this(units)
        {
            Quantity *= quantity;
        }

        internal ComplexUnit(double quantity = 1, Prefix prefix = 0, string unitName = null, int power = 1, params Unit[] units)
            : this(quantity, units)
        {
            UnitName = unitName;
            _prefix = prefix;
            _power = power;
        }

        #endregion
        #region Methods
        public static Unit Multiply(params Unit[] units)
        {
            var quantity = units.Aggregate(1.0, (x, y) => x * y.Quantity);
            var groups = units.SelectMany(x => x).GroupBy(x => x.UnitName);
            var multiplied = groups.AsParallel().Select(group => group.Aggregate((x, y) => x * y)).ToArray();
            return new ComplexUnit(quantity, multiplied);
        }

        private static IList<Unit> OrderUnits(IEnumerable<Unit> units)
        {
            return /*new ReadOnlyCollection<Unit>(*/units.OrderByDescending(x => Math.Sign(x.Power)).ThenBy(x => x.UnitName).ToArray()/*)*/;
        }

        private void unit_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            int i = (int)_prefix + (int)sender;
            Prefix newPrefix = PrefixHelpers.FindClosestPrefix(i);
            _prefix = newPrefix;
        }

        public override Unit Pow(int power)
        {
            var quantity = Math.Pow(Quantity, power);
            var newPower = Power * power;
            var units = Units.Select(x => x.Pow(power)).ToArray();
            var unit = new ComplexUnit(quantity, Prefix, UnitName, newPower, units);
            return unit;
        }

        public void FindDerivedUnits()
        {
            if (string.IsNullOrEmpty(UnitName))
            {
                var units = new List<Unit>();
                ComplexUnit d = null;
                ComplexUnit r = this;
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

        private bool FindDerivedUnitWithSmallestRemain(ref ComplexUnit derived, ref ComplexUnit remain)
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

        private IDictionary<ComplexUnit,ComplexUnit> FindPossibleDerivedUnits()
        {
            var dict = new ConcurrentDictionary<ComplexUnit, ComplexUnit>();
            DerivedUnits.AsParallel().ForAll(derivedUnit =>
            {
                int factor = 0;
                var remain = this.HasFactor(derivedUnit, ref factor) as ComplexUnit;
                if (factor != 0)
                {      
                    var pow10 = PrefixHelpers.Power10(remain.Quantity);
                    var prfx = PrefixHelpers.FindClosestPrefix(pow10 / factor);
                    var units = derivedUnit.Units.SelectMany(x=> x.Pow(1)).ToArray();
                    var quantity = remain.Quantity / Math.Pow(10, (int)prfx * factor);
                    ComplexUnit d = new ComplexUnit(quantity, prfx, derivedUnit.UnitName, factor, units);
                    dict.AddOrUpdate(d, remain, (x, y) => y);
                }
            });
            return dict;
        }

        public override Unit HasFactor(Unit unit, ref int factor)
        {
            var u = Factor(unit, ref factor);
            if (factor == 0)
            {
                u = Factor(unit.Pow(-1), ref factor);
                factor *= -1;
            }
            return u;
        }

        private Unit Factor(Unit possibleFactor, ref int factor)
        {
            var remain = this;
            bool repeat = false;
            do
            {
                var quotient = remain / possibleFactor;
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
                    remain = quotient as ComplexUnit;
                    factor++;
                }
            }
            while (repeat);
            return remain;
        }

        public override bool IsAddable(Unit u)
        {
            var u1 = u as ComplexUnit;
            if (u1 != null && Power == u.Power)
            {
                var a = Units.SelectMany(x => x).Select(x => new { x.Power, x.UnitName });
                var b = u1.Units.SelectMany(x => x).Select(x => new { x.Power, x.UnitName });
                bool addable = !a.Except(b).Any();
                if (addable)
                    return true;
            }
            return false;
        }

        public override IEnumerator<Unit> GetEnumerator()
        {
            foreach (var unit in Units)
            {
                yield return unit;
            }
        }

        public override string ToString()
        {
            return ToString("");
        }

        public override string ToString(string format)
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

        #endregion
    }
}
