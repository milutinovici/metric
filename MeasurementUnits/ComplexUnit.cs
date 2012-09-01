using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasurementUnits
{
    public class ComplexUnit : Unit
    {
        #region Properties
        public IEnumerable<Unit> Units { get; set; }
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
                Units = Units.Select(x => x.Pow(quotient));
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
                var s = Units.OrderBy(x => Math.Abs((int)x.Prefix)).ThenBy(x => Math.Abs(x.Power)).FirstOrDefault(x=> pvalue % x.Power == 0);
                s.Prefix += pvalue / s.Power;
                base.Prefix = value;
            }
        }
        public string DerivedUnit { get; private set; }
        #region Derived Units
        internal static readonly IEnumerable<ComplexUnit> DerivedUnits = new List<ComplexUnit> 
        {
            new ComplexUnit("Ω", new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -3), new Unit(BaseUnit.A, -2)),
            new ComplexUnit("V", new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -3), new Unit(BaseUnit.A, -1)),
            new ComplexUnit("H", new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s,-2), new Unit(BaseUnit.A, -2)),
            new ComplexUnit("Wb", new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -2), new Unit(BaseUnit.A, -1)),

            new ComplexUnit("F", new Unit(Prefix.k, BaseUnit.g, -1), new Unit(BaseUnit.m, -2), new Unit(BaseUnit.s, 4), new Unit(BaseUnit.A, 2)),
            new ComplexUnit("S", new Unit(Prefix.k, BaseUnit.g, -1), new Unit(BaseUnit.m, -2), new Unit(BaseUnit.s, 3), new Unit(BaseUnit.A, 2)),

            new ComplexUnit("W", new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -3)),
            new ComplexUnit("J", new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -2)),
            new ComplexUnit("N", new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m), new Unit(BaseUnit.s, -2)),
            
            new ComplexUnit("Pa", new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, -1), new Unit(BaseUnit.s, -2)),
            new ComplexUnit("T", new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.s, -2), new Unit(BaseUnit.A, -1)),
            
            new ComplexUnit("C", new Unit(BaseUnit.s), new Unit(BaseUnit.A)),
            new ComplexUnit("Gy", new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -2)),
            new ComplexUnit("lx", new Unit(BaseUnit.m, -2), new Unit(BaseUnit.cd)),
            new ComplexUnit("kat", new Unit(BaseUnit.s, -1), new Unit(BaseUnit.mol)),
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

        internal ComplexUnit(string derivedUnit, params Unit[] units)
            : this(units)
        {
            DerivedUnit = derivedUnit;
        }
        internal ComplexUnit(double quantity, string derivedUnit, params Unit[] units)
            : this(quantity, units)
        {
            DerivedUnit = derivedUnit;
        }
        internal ComplexUnit(Prefix prefix, string derivedUnit, int power, params Unit[] units)
            : this(derivedUnit, units)
        {
            _prefix = prefix;
            _power = power;
        }
        internal ComplexUnit(double quantity, Prefix prefix, string derivedUnit, int power, params Unit[] units)
            : this(quantity, derivedUnit, units)
        {
            _prefix = prefix;
            _power = power;
        }

        #endregion
        #region Methods
        public static Unit Multiply(params Unit[] units)
        {
            var quantity = units.Aggregate(1.0, (x, y) => x * y.Quantity);
            var groups = units.SelectMany(x => x).GroupBy(x => x.BaseUnit);
            var multiplied = groups.AsParallel().Select(group => group.Aggregate((x, y) => x * y)).ToArray();
            return new ComplexUnit(quantity, multiplied);
        }

        private static IEnumerable<Unit> OrderUnits(IEnumerable<Unit> units)
        {
            return units.OrderByDescending(x => Math.Sign(x.Power)).ThenBy(x => x.BaseUnit);
        }

        

        private void unit_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            int i = (int)_prefix + (int)sender;
            Prefix newPrefix = PrefixHelpers.FindClosestPrefix(i);
            _prefix = newPrefix;
        }

        public override Unit Pow(int power)
        {
            if (DerivedUnit == null)
            {
                var unit = new ComplexUnit(Units.Select(x => x.Pow(power)).ToArray());
                unit.Quantity = Math.Pow(Quantity, power);
                return unit;
            }
            else
            {
                var unit = new ComplexUnit(Prefix, DerivedUnit, Power, Units.ToArray());
                unit.Power *= power;
                unit.Quantity = Math.Pow(Quantity, power);
                return unit;
            }
        }

        public void FindDerivedUnits()
        {
            if (DerivedUnit == null || DerivedUnit == "")
            {
                var units = new List<Unit>();
                ComplexUnit d = null;
                ComplexUnit r = this;
                while (r.Units.Any() && r.FindDerivedUnitWithSmallestRemain(ref d, ref r))
                {
                    units.Add(d);
                }
                if (r.Units.Count() != 0 || r.BaseUnit != 0)
                {
                    units.AddRange(r.SelectMany(x => x));
                }
                Quantity = d != null ? d.Quantity : r.Quantity;
                Units =  OrderUnits(units);
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
            Parallel.ForEach(DerivedUnits, derivedUnit =>
            {
                int factor = 0;
                var remain = this.HasFactor(derivedUnit, ref factor) as ComplexUnit;
                if (factor != 0)
                {      
                    var pow10 = PrefixHelpers.Power10(remain.Quantity);
                    var prfx = PrefixHelpers.FindClosestPrefix(pow10 / factor);
                    ComplexUnit d = new ComplexUnit(prfx, derivedUnit.DerivedUnit, factor, GetBySymbol(derivedUnit.DerivedUnit));
                    d.Quantity = remain.Quantity / Math.Pow(10, (int)prfx * factor);
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
                var quotients = this.SelectMany(x => x).Concat(quotient.SelectMany(x => x)).GroupBy(x => x.BaseUnit);
                var factors = this.SelectMany(x => x).Concat(possibleFactor.SelectMany(x => x)).GroupBy(x => x.BaseUnit);
                
                var qgroup = quotients.Where(group => group.Count() > 1);
                var fgroup = factors.Where(group => group.Count() > 1);
                var rgroup = quotients.Where(group => group.Count() < 2).SelectMany(x => x).Concat(factors.Where(group => group.Count() < 2).SelectMany(x => x))
                    .GroupBy(x => x.BaseUnit).Where(group => group.Count() > 1);

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
            bool findDerived = DerivedUnit == null && !format.Contains("b");//base units only
            if (findDerived) FindDerivedUnits();
            bool fancy = !format.Contains("c");//common
            bool negativeExponent = !format.Contains("d");// use '/'
            string quantity = !format.Contains("i") ? Quantity.ToString() : ""; // display quanttity
            if (DerivedUnit == null)
            {
                if (Units.Count() == 0)
                {
                    return "";
                }
                StringBuilder name = new StringBuilder();
                string multiplier = fancy ? Stringifier.dot : "*";
                string f = fancy ? "" : "c"; f += "i";
                var group = Units.GroupBy(x => Math.Sign(x.Power));
                foreach (var unit in group.ElementAt(0))
                {
                    name.Append(unit.ToString(f)).Append(multiplier);
                }
                if (group.Count() > 1)
                {
                    if (negativeExponent)
                    {
                        foreach (var unit in group.ElementAt(1))
                        {
                            name.Append(unit.ToString(f)).Append(multiplier);
                        }
                    }
                    else
                    {
                        name.Remove(name.Length - 1, 1);
                        name.Append("/");
                        foreach (var unit in group.ElementAt(1))
                        {
                            name.Append(unit.Pow(-1).ToString(f)).Append(multiplier);
                        }
                    }
                }
                return quantity + name.Remove(name.Length - 1, 1).ToString();
            }
            else
            {
                return quantity + Stringifier.UnitToString(Prefix, DerivedUnit, Power, fancy);
            }
        }

        public override bool IsAddable(Unit u)
        {
            var u1 = u as ComplexUnit;
            if (u1 != null && Power == u.Power)
            {
                var a = Units.SelectMany(x => x).Select(x => new { x.Power, x.BaseUnit });
                var b = u1.Units.SelectMany(x => x).Select(x => new { x.Power, x.BaseUnit });
                bool addable = !a.Except(b).Any();
                if (addable)
                    return true;
            }
            return false;
        }

        #endregion
    }
}
