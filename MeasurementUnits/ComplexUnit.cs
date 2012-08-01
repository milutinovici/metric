using System;
using System.Collections.Generic;
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
                var s = Units.OrderBy(x => Math.Abs((int)x.Prefix)).ThenBy(x => Math.Abs(x.Power)).FirstOrDefault();
                s.Prefix += pvalue / s.Power;
                base.Prefix = value;
            }
        }
        public string DerivedUnit { get; private set; }
        private static readonly IEnumerable<ComplexUnit> DerivedUnits = new List<ComplexUnit> 
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
        #region Constructors
        protected ComplexUnit()
        {
            Units = new List<Unit>();
        }
        public ComplexUnit(params Unit[] units)
            : this()
        {
            Units = OrderUnits(units);
            foreach (var unit in Units)
            {
                unit.PropertyChanged += unit_PropertyChanged;
            }
        }
        protected ComplexUnit(int power10, params Unit[] units)
            : this(units)
        {
            Power10 = power10;
        }
        protected ComplexUnit(string derivedUnit, params Unit[] units)
            : this(units)
        {
            DerivedUnit = derivedUnit;
        }
        protected ComplexUnit(string derivedUnit, int power, params Unit[] units)
            : this(derivedUnit, units)
        {
            _power = power;
        }
        protected ComplexUnit(string derivedUnit, Prefix prefix, int power, params Unit[] units)
            : this(derivedUnit, power, units)
        {
            _prefix = prefix;
        }
        #endregion
        #region Methods
        public static Unit Multiply(params Unit[] units)
        {
            var unts = new List<Unit>();
            int power10 = 0;
            var groups = units.SelectMany(x => x).GroupBy(x => x.BaseUnit);
            foreach (var group in groups)
            {
                var baseUnit = group.Aggregate((x, y) => x * y);
                power10 += baseUnit.Power10;
                baseUnit.Power10 = 0;
                if (baseUnit.Power != 0)
                    unts.Add(baseUnit);
            }
            return new ComplexUnit(power10, unts.ToArray());
        }

        private static IEnumerable<Unit> OrderUnits(IEnumerable<Unit> units)
        {
            return units.OrderByDescending(x => Math.Sign(x.Power)).ThenBy(x => x.BaseUnit);
        }

        private void unit_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            int i = (int)_prefix + (int)sender;
            Prefix newPrefix = Unit.FindClosestPrefix(i);
            Power10 += (int)newPrefix - i;
            _prefix = newPrefix;
        }
        public static ComplexUnit GetBySymbol(string symbol)
        {
            var derived = DerivedUnits.First(x => x.DerivedUnit == symbol).SelectMany(x => x.Pow(1)).ToArray();
            return new ComplexUnit(symbol, derived);
        }
        public void FindDerivedUnits()
        {
            if (DerivedUnit == null || DerivedUnit == "")
            {
                var units = new List<Unit>();
                ComplexUnit d = null;
                ComplexUnit r = this;
                while (r.FindDerivedUnitWithSmallestRemain(ref d, ref r))
                {
                    units.Add(d);
                }
                if (r.Units.Count() != 0 || r.BaseUnit != 0)
                {
                    units.AddRange(r.SelectMany(x => x));
                }
                Units =  OrderUnits(units);
                this.Power10 += r.Power10;
            }
        }
        internal bool FindDerivedUnitWithSmallestRemain(ref ComplexUnit derived, ref ComplexUnit remain)
        {
            var dict = new Dictionary<ComplexUnit, ComplexUnit>();
            foreach (ComplexUnit derivedUnit in DerivedUnits)
            {
                int factor = 0;
                var r = this.HasFactor(derivedUnit, ref factor) as ComplexUnit;
                if (factor != 0)
                {
                    var remainPower = r.Power10 * factor;
                    var testDerived = (this / r);
                    var prfx = Unit.FindClosestPrefix(remainPower);
                    ComplexUnit d = new ComplexUnit(derivedUnit.DerivedUnit, prfx, factor, testDerived);
                    r.Power10 = remainPower - (int)d.Prefix;
                    dict.Add(d, r);
                }
            }
            if (dict.Any())
            {
                var optimal = dict.OrderBy(x => x.Value.Count()).First();          
                derived = optimal.Key;  
                remain = optimal.Value;
                return true;
            }
            else return false;

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
        public override Unit Pow(int power)
        {
            if (DerivedUnit == null)
            {
                return new ComplexUnit(Units.Select(x => x.Pow(power)).ToArray());
            }
            else
            {
                var unit = new ComplexUnit(DerivedUnit, Prefix, Power ,Units.ToArray());
                unit.Power *= power;
                return unit;
            }

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
            bool fancy = !format.Contains("c");
            bool negativeExponent = !format.Contains("d");
            if (DerivedUnit == null)
            {
                if (Units.Count() == 0)
                {
                    return "";
                }
                StringBuilder name = new StringBuilder();
                string multiplier = fancy ? Str.dot : "*";
                string f = fancy ? "" : "c";
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
                return name.Remove(name.Length - 1, 1).ToString();
            }
            else
            {
                return Str.UnitToString(Prefix, DerivedUnit, Power, fancy);
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
