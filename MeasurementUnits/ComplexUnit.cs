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
            new ComplexUnit("N", new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m), new Unit(BaseUnit.s, -2)),
            new ComplexUnit("Pa", new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, -1), new Unit(BaseUnit.s, -2)),
            new ComplexUnit("J", new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -2)),
            new ComplexUnit("W", new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -3)),
            new ComplexUnit("C", new Unit(BaseUnit.s), new Unit(BaseUnit.A)),
            new ComplexUnit("V", new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -3), new Unit(BaseUnit.A, -1)),
            new ComplexUnit("F", new Unit(Prefix.k, BaseUnit.g, -1), new Unit(BaseUnit.m, -2), new Unit(BaseUnit.s, 4), new Unit(BaseUnit.A, 2)),
            new ComplexUnit("Ω", new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -3), new Unit(BaseUnit.A, -2)),
            new ComplexUnit("S", new Unit(Prefix.k, BaseUnit.g, -1), new Unit(BaseUnit.m, -2), new Unit(BaseUnit.s, 3), new Unit(BaseUnit.A, 2)),
            new ComplexUnit("Wb", new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -2), new Unit(BaseUnit.A, -1)),
            new ComplexUnit("T", new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.s, -2), new Unit(BaseUnit.A, -1)),
            new ComplexUnit("H", new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s,-2), new Unit(BaseUnit.A, -2))
        };
        #endregion
        #region Constructors
        public ComplexUnit()
        {
            Units = new List<Unit>();
        }
        public ComplexUnit(params Unit[] units)
            : this()
        {
            Units = units;
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
            return new ComplexUnit(power10, unts.OrderByDescending(x => Math.Sign(x.Power)).ThenBy(x => x.BaseUnit).ToArray());
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
            var empty = new Unit(BaseUnit.m, 0);
            return empty * DerivedUnits.First(x => x.DerivedUnit == symbol) as ComplexUnit;
        }
        public void FindDerivedUnits()
        {
            var units = new List<Unit>();
            ComplexUnit remain = this;
            int remainPower = remain.Power10;
            foreach (ComplexUnit derivedUnit in DerivedUnits)
            {
                int factor = 0;
                remain = remain.HasFactor(derivedUnit, ref factor) as ComplexUnit;
                if (factor != 0)
                {
                    remainPower = remain.Power10;
                    remainPower *= factor;
                    var derr = (this / remain);
                    var prfx = Unit.FindClosestPrefix(remainPower);
                    remainPower -= (int)prfx;
                    ComplexUnit der = new ComplexUnit(derivedUnit.DerivedUnit, prfx, factor, derr);
                    units.Add(der);
                }
            } 
            if (remain.Units.Count() != 0 || remain.BaseUnit != 0)
            {
                units.AddRange(remain.SelectMany(x=>x));
            }
            Units = units;
            this.Power10 += remainPower;
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
                var unit = new ComplexUnit(DerivedUnit, Units.ToArray());
                unit.Power = power;
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
            if (DerivedUnit == null)
            {
                if (Units.Count() == 0)
                {
                    return "";
                }
                StringBuilder name = new StringBuilder();
                foreach (var unit in Units)
                {
                    name.Append(unit.ToString()).Append(Str.dot);
                }
                return name.Remove(name.Length - 1, 1).ToString();
            }
            else
            {
                if(Prefix == 0)
                    return Power == 1 ? DerivedUnit : DerivedUnit + Str.SS(Power);
                else
                    return Power == 1 ? Prefix + DerivedUnit : Prefix + DerivedUnit + Str.SS(Power);
            }
        }
        public override bool IsAddable(Unit u)
        {
            var u1 = u as ComplexUnit;
            if (u1 != null)
            {
                if (Power == u.Power && Units.SequenceEqual(u1.Units))
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        #endregion
        
    }
}
