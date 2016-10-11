using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace Metric
{
    public enum BaseUnit : sbyte { m = 1, g, s, A, K, cd, mol }
    
    public struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable, IFormattable
    {
        static int Length = Vector<sbyte>.Count;
        Vector<sbyte> Powers { get; } 
        Vector<sbyte> Prefixes { get; }
        public double Quantity { get; }
        
        static readonly IDictionary<string, Unit> DerivedUnits = new Dictionary<string, Unit>
        {
            ["Ω"] = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s, -3) * new Unit(1, BaseUnit.A, -2),
            ["V"] = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s, -3) * new Unit(1, BaseUnit.A, -1),
            ["H"] = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s, -2) * new Unit(1, BaseUnit.A, -2),
            ["Wb"] = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s, -2) * new Unit(1, BaseUnit.A, -1),

            ["F"] = new Unit(1, Prefix.k, BaseUnit.g, -1) * new Unit(1, BaseUnit.m, -2) * new Unit(1, BaseUnit.s, 4) * new Unit(1, BaseUnit.A, 2),
            ["S"] = new Unit(1, Prefix.k, BaseUnit.g, -1) * new Unit(1, BaseUnit.m, -2) * new Unit(1, BaseUnit.s, 3) * new Unit(1, BaseUnit.A, 2),

            ["W"] = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s, -3),
            ["J"] = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s, -2),
            ["N"] = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m) * new Unit(1, BaseUnit.s, -2),

            ["Pa"] = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m, -1) * new Unit(1, BaseUnit.s, -2),
            ["T"] = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.s, -2) * new Unit(1, BaseUnit.A, -1),

            ["C"] = new Unit(1, BaseUnit.s) * new Unit(1, BaseUnit.A),
            ["Gy"] = new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s, -2),
            ["lx"] = new Unit(1, BaseUnit.m, -2) * new Unit(1, BaseUnit.cd),
            ["kat"] = new Unit(1, BaseUnit.s, -1) * new Unit(1, BaseUnit.mol),

            //new Unit(60, BaseUnit.s),
        };

        public Unit(double quantity, BaseUnit baseUnit, sbyte power = 1)
        {
            
            this.Prefixes = new Vector<sbyte>();
            if(baseUnit > 0)
            {
                var powers = new sbyte[Length];
                powers[(sbyte)baseUnit - 1] = power;
                this.Powers = new Vector<sbyte>(powers);
            }
            this.Quantity = quantity;
        }
        public Unit(double quantity, Prefix prefix, BaseUnit baseUnit, sbyte power = 1)
            : this(quantity, baseUnit, power)
        {
            var prefixes = new sbyte[Length];
                prefixes[(sbyte)baseUnit - 1] = (sbyte)prefix;
            this.Prefixes = new Vector<sbyte>(prefixes);

        }
        Unit(double quantity, Vector<sbyte> prefixes, Vector<sbyte> powers)
        {
            this.Quantity = quantity;
            this.Powers = powers;
            this.Prefixes = prefixes;        
        }
        /// <summary>
        /// Checks wether a unit has a factor and returns its power. e.g. 'm/s^2' returns -2 with 's' as a parameter. Returns 0 if parameter is not its factor. 
        /// </summary>
        /// <param name="factor">The factor u want to check</param>
        /// <returns>Power of the provided factor</returns>
        public sbyte HasFactor(Unit factor)
        {
            sbyte? power = null;
            for(sbyte i = 0; i < Length; i++)
            {
                if(factor.Powers[i] != 0)
                {
                    var temp = (sbyte)(Powers[i] / factor.Powers[i]);
                    if(temp == 0 || (power.HasValue && Math.Sign(power.Value * temp) == -1))
                    {
                        return 0;
                    }
                    else if(power == null || Math.Abs(temp) < Math.Abs(power.Value))
                    {
                        power = temp;
                    }
                }
            }
            return power ?? 0;
        }
        /// <summary>
        /// Returns a new unit with the specified power. 
        /// </summary>
        /// <param name="power">power</param>
        /// <exception>You can have only integer powers. You cannot power m^3 with 0.5, because m^1.5 doesn't make sense
        ///   <cref>DimensionSplitException</cref>
        /// </exception>
        /// <returns>new Unit</returns>
        public Unit Pow(double power)
        {
            if(Math.Abs(power) < 1)
            {
                var reciprocal = (sbyte)(1/power);
                var powers = new sbyte[Length];
                for(sbyte i = 0; i < Length; i++) {
                    if(Powers[i] % reciprocal != 0) {
                        throw new DimensionSplitException(this, power);
                    }
                    powers[i] = (sbyte)(Powers[i] / reciprocal);
                }
                return new Unit(Math.Pow(Quantity, power), Prefixes, new Vector<sbyte>(powers));
            }
            return new Unit(Math.Pow(Quantity, power), Prefixes, Powers * (sbyte)power);
        }
        /// <summary>
        /// Checks whether a unit with a given symbol exists
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>bool</returns>
        public static bool Exists(string symbol)
        {
            BaseUnit bu;
            return Enum.TryParse(symbol, out bu) || DerivedUnits.ContainsKey(symbol);
        }
        /// <summary>
        /// Get a single Unit, base or derived, using the specified symbol
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>new Unit</returns>
        public static Unit Create(string symbol)
        {
            BaseUnit bu;
            bool success = Enum.TryParse(symbol, out bu);
            if (success)
            {
                return new Unit(1, bu);
            }
            return DerivedUnits[symbol];
        }
        /// <summary>
        /// Get a single prefixed Unit, base or derived, using the specified symbol
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static Unit Create(Prefix prefix, string symbol)
        {
            BaseUnit bu;
            bool success = Enum.TryParse(symbol, out bu);
            if (success)
            {
                return new Unit(1, prefix, bu);
            }
            var powers = DerivedUnits[symbol].Powers;
            for(sbyte i = 0; i < Length; i++) {
                if(powers[i] != 0 && (sbyte)prefix % powers[i] == 0) {
                    var prefixes = new sbyte[Length];
                    DerivedUnits[symbol].Prefixes.CopyTo(prefixes, 0);
                    prefixes[i] += (sbyte)((sbyte)(prefix) / powers[i]);
                    return new Unit(1, new Vector<sbyte>(prefixes), powers);
                }
            }
            return new Unit(Math.Pow(10, (sbyte)prefix), DerivedUnits[symbol].Prefixes, powers);
        }
        /// <summary>
        /// Parse specified string to Unit
        /// </summary>
        /// <param name="s"></param>
        /// <returns>new Unit</returns>
        public static Unit Parse(string s)
        {
            return UnitParser.Parse(s);
        }
        /// <summary>
        /// Try to parse specified string to Unit
        /// </summary>
        /// <param name="s"></param>
        /// <param name="unit"></param>
        /// <returns>new Unit</returns>
        public static bool TryParse(string s, out Unit unit)
        {
            try
            {
                unit = UnitParser.Parse(s);
                return true;
            }
            catch
            {
                unit = new Unit(1, 0);
                return false;
            }
        }
        /// <summary>
        /// Returns a new unit with the specified prefix
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns>new Unit</returns>
        public Unit ChangePrefix(Prefix prefix, BaseUnit baseUnit)
        {
            int difference = ((sbyte)GetPrefix(baseUnit) - (sbyte)prefix) * GetPower(baseUnit);
            var prefixes = new sbyte[Length];
            Prefixes.CopyTo(prefixes, 0);
            prefixes[(sbyte)baseUnit - 1] = (sbyte)prefix;
            double quantity = Quantity * Math.Pow(10, difference);
            return new Unit(quantity, new Vector<sbyte>(prefixes), Powers);
        }
        /// <summary>
        /// Get the prefix of the specified base unit
        /// </summary>
        /// <param name="baseUnit"></param>
        /// <returns>Prefix</returns>
        public Prefix GetPrefix(BaseUnit baseUnit)
        {
            var index = (sbyte)baseUnit;
            if (index > 0 && index <= Length)
                return (Prefix)Prefixes[index - 1];
            else return 0;
        }
        /// <summary>
        /// get the power of the specified base unit
        /// </summary>
        /// <param name="baseUnit"></param>
        /// <returns>sbyte</returns>
        public sbyte GetPower(BaseUnit baseUnit)
        {
            if (baseUnit > 0 && (sbyte)baseUnit < Length)
                return Powers[(sbyte)baseUnit - 1];
            else return 0;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Unit)) {
                return false;
            }
            return Equals((Unit)obj);
        }
        public bool Equals(Unit other)
        {
            if (IsComparable(other))
            {
                var power = Power10Difference(other);
                return Math.Abs(Math.Pow(10, power) * Quantity - other.Quantity) < double.Epsilon;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return Quantity.GetHashCode() * Prefixes.GetHashCode() * Powers.GetHashCode();
        }
        public bool IsComparable(Unit other)
        {
            return Powers == other.Powers;
        }
        public int CompareTo(Unit other)
        {
            if (IsComparable(other) == false)
            {
                throw new IncomparableUnitsException(this, other);
            }
            var power = Power10Difference(other);
            return (Math.Pow(10, power) * Quantity).CompareTo(other.Quantity);
        }
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            if (!(obj is Unit))
            {
                throw new IncomparableUnitsException(this, obj);
            }
            return CompareTo((Unit)obj);
        }
        public override string ToString()
        {
            return ToString(null, System.Globalization.CultureInfo.CurrentCulture);
        }
        public string ToString(string format)
        {
            return ToString(format, System.Globalization.CultureInfo.CurrentCulture);
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            format = (format ?? "");
            bool fancy = !ContainsIgnoreCase(format, "C"); // common formmating 
            bool useDivisor = ContainsIgnoreCase(format, "D"); // use '/'
            bool baseOnly = ContainsIgnoreCase(format, "B"); // base units only
            string dot = fancy ? SingleUnit.Dot : "*";

            double quantity;
            IEnumerable<SingleUnit> singles;
            if (baseOnly)
            {
                var self = this;
                quantity = self.Quantity;
                singles = GetSingleUnits();
            }
            else
            {
                singles = FindDerivedUnits(this, out quantity);
            }
            var numerator = new StringBuilder();
            var denominator = new StringBuilder();

            foreach(var single in singles) 
            {
                if(!useDivisor || (useDivisor && Math.Sign(single.Power) == 1)) 
                {
                    if(numerator.Length != 0)
                    {
                        numerator.Append(dot);
                    }
                    numerator.Append(single.ToString(fancy));
                }
                else if(useDivisor)
                {
                    if(denominator.Length != 0)
                    {
                        denominator.Append(dot);
                    }
                    denominator.Append(single.Reciprocal().ToString(fancy));
                }
            }
            if (denominator.Length > 0 && numerator.Length > 0)
            {
                return $"{quantity.ToString(formatProvider)}{numerator.ToString()}/{denominator.ToString()}";
            }
            else if(denominator.Length > 0)
            {
                return $"{quantity.ToString(formatProvider)}/{denominator.ToString()}";
            }
            else 
            {
                return $"{quantity.ToString(formatProvider)}{numerator.ToString()}";
            }

        }

        static IEnumerable<SingleUnit> FindDerivedUnits(Unit unit, out double q)
        {
            q = 1;
            var optimal = new KeyValuePair<string, Unit>(null, new Unit());
            sbyte optimalPower = 0;
            short lastSum = 0;
            foreach(var derived in DerivedUnits)
            {
                var power = unit.HasFactor(derived.Value);
                if(power != 0)
                {
                    short sum = 0;
                    for(sbyte i = 0; i < Length; i++) {
                        sum += Math.Abs(derived.Value.Powers[i]);
                    }
                    sum *= Math.Abs(power);
                    if(sum > lastSum)
                    {
                        optimal = derived;
                        lastSum = sum;
                        optimalPower = power;
                    }
                }
            }

            if (optimal.Key != null)
            {
                Unit remainder = unit / optimal.Value.Pow(optimalPower);
                var pow10 = PrefixHelpers.Power10(remainder.Quantity);
                var prfx = PrefixHelpers.FindClosestPrefix(pow10 / optimalPower);
                var quantity = remainder.Quantity / Math.Pow(10, (int)prfx * optimalPower);
                var list = new List<SingleUnit>();
                list.Add(new SingleUnit(prfx, optimal.Key, optimalPower));
                var more = FindDerivedUnits(remainder.NewQuantity(quantity), out q);
                list.AddRange(more);
                return list;
            }
            else
            {
                q = unit.Quantity;
                return unit.GetSingleUnits();
            }
        }
        IEnumerable<SingleUnit> GetSingleUnits() {
            for(sbyte i = 0; i < Length; i++) {
                if(Powers[i] != 0) {
                    var baseUnit = (BaseUnit)(i + 1);
                    yield return new SingleUnit((Prefix)Prefixes[i], baseUnit.ToString(), Powers[i]);
                }
            }
        }
        Unit NewQuantity(double quantity)
        {
            return new Unit(quantity, Prefixes, Powers);
        }
        int Power10Difference(Unit other)
        {
            int diff = 0;
            for (int i = 0; i < Length; i++)
            {
                if (Powers[i] != 0 && other.Powers[i] != 0)
                {
                    diff += (sbyte)Prefixes[i] * Powers[i] - (sbyte)other.Prefixes[i] * other.Powers[i];
                }
            }
            return diff;
        }
        Vector<sbyte> PrefixMerge(Unit other, out int diff1, out int diff2, bool? multiplication)
        {
            var result = new sbyte[Length];
            diff1 = 0;
            diff2 = 0;
            for (int i = 0; i < Length; i++)
            {
                //reduction
                if ((multiplication == true && Powers[i] + other.Powers[i] == 0) || (multiplication == false && Powers[i] - other.Powers[i] == 0))
                {
                    result[i] = 0;
                    diff2 += ((sbyte)result[i] - (sbyte)other.Prefixes[i]) * other.Powers[i];
                    diff1 += ((sbyte)Prefixes[i] - (sbyte)result[i]) * Powers[i];
                }
                else if (Prefixes[i] != 0)
                {
                    result[i] = Prefixes[i];
                    diff2 += ((sbyte)result[i] - (sbyte)other.Prefixes[i]) * other.Powers[i];
                }
                else
                {
                    result[i] = other.Prefixes[i];
                    diff1 += ((sbyte)Prefixes[i] - (sbyte)result[i]) * Powers[i];
                }
            }
            return new Vector<sbyte>(result);
        }
        static bool ContainsIgnoreCase(string corpus, string key)
        {
            return CultureInfo.CurrentCulture.CompareInfo.IndexOf(corpus, key, CompareOptions.IgnoreCase) >= 0;
        }
        #region Operators
        public static Unit operator +(Unit u1)
        {
            return u1;
        }
        public static Unit operator -(Unit u1)
        {
            return u1.NewQuantity(-u1.Quantity);
        }
        public static Unit operator ++(Unit u1)
        {
            return u1.NewQuantity(u1.Quantity + 1);
        }
        public static Unit operator --(Unit u1)
        {
            return u1.NewQuantity(u1.Quantity - 1);
        }
        public static Unit operator +(Unit u1, Unit u2)
        {
            if (u1.IsComparable(u2))
            {
                var p1 = 0;
                var p2 = 0;
                var prefixes = u1.PrefixMerge(u2, out p1, out p2, null);
                var quantity = u1.Quantity * Math.Pow(10, p1) + u2.Quantity * Math.Pow(10, p2);
                var newUnit = new Unit(quantity, prefixes, u1.Powers);
                return newUnit;
            }
            throw new IncomparableUnitsException(u1, u2);
        }

        public static Unit operator -(Unit u1, Unit u2)
        {
            if (u1.IsComparable(u2))
            {
                var p1 = 0;
                var p2 = 0;
                var prefixes = u1.PrefixMerge(u2, out p1, out p2, null);
                var quantity = u1.Quantity * Math.Pow(10, p1) - u2.Quantity * Math.Pow(10, p2);
                var newUnit = new Unit(quantity, prefixes, u1.Powers);
                return newUnit;
            }
            throw new IncomparableUnitsException(u1, u2);
        }

        public static Unit operator *(Unit u1, Unit u2)
        {
            var p1 = 0;
            var p2 = 0;
            var prefixes = u1.PrefixMerge(u2, out p1, out p2, true);
            var powers = u1.Powers + u2.Powers;
            var quantity = u1.Quantity * u2.Quantity * Math.Pow(10, p1) * Math.Pow(10, p2);
            return new Unit(quantity, prefixes, powers);
        }

        public static Unit operator /(Unit u1, Unit u2)
        {
            var p1 = 0;
            var p2 = 0;
            var prefixes = u1.PrefixMerge(u2, out p1, out p2, false);
            var powers = u1.Powers - u2.Powers;
            var quantity = u1.Quantity * Math.Pow(10, p1) / u2.Quantity * Math.Pow(10, p2);
            return new Unit(quantity, prefixes, powers);
        }

        public static Unit operator +(double number, Unit unit)
        {
            return unit.NewQuantity(unit.Quantity + number);
        }
        public static Unit operator +(Unit unit, double number)
        {
            return number + unit;
        }
        public static Unit operator -(double number, Unit unit)
        {
            return unit.NewQuantity(number - unit.Quantity);
        }
        public static Unit operator -(Unit unit, double number)
        {
            return unit.NewQuantity(unit.Quantity - number);
        }
        public static Unit operator *(double number, Unit unit)
        {
            return unit.NewQuantity(unit.Quantity * number);
        }
        public static Unit operator *(Unit unit, double number)
        {
            return number * unit;
        }
        public static Unit operator /(double number, Unit unit)
        {
            return unit.Pow(-1).NewQuantity(number / unit.Quantity);
        }
        public static Unit operator /(Unit unit, double number)
        {
            return unit.NewQuantity(unit.Quantity / number);
        }
        public static bool operator ==(Unit u1, Unit u2)
        {
            return Equals(u1, u2);
        }
        public static bool operator !=(Unit u1, Unit u2)
        {
            return !Equals(u1, u2);
        }
        public static bool operator <(Unit u1, Unit u2)
        {
            return u1.CompareTo(u2) < 0;
        }
        public static bool operator >(Unit u1, Unit u2)
        {
            return u1.CompareTo(u2) > 0;
        }
        public static bool operator <=(Unit u1, Unit u2)
        {
            return u1.CompareTo(u2) <= 0;
        }
        public static bool operator >=(Unit u1, Unit u2)
        {
            return u1.CompareTo(u2) >= 0;
        }
        public static explicit operator double(Unit u)
        {
            return u.Quantity;
        }
        public static explicit operator Unit(double u)
        {
            return new Unit(u, 0);
        }
        #endregion

    }

}
