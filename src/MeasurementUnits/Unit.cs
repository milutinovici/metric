using System;
using System.Collections.Generic;
using System.Linq;


namespace MeasurementUnits
{
    public enum BaseUnit : sbyte { m = 1, g, s, A, K, cd, mol }
    
    public struct Unit : IEnumerable<Unit>, IEquatable<Unit>, IComparable<Unit>, IComparable
    {
        sbyte[] Powers { get; } 
        Prefix[] Prefixes { get; }
        public double Quantity { get; }
        public string Symbol { get; }


        private static readonly IDictionary<string, Unit> DerivedUnits = new Dictionary<string, Unit> 
        {
            //  ["Ω"] = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s, -3) * new Unit(1, BaseUnit.A, -2),
            //  ["V"] = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s, -3) * new Unit(1, BaseUnit.A, -1),
            //  ["H"] = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s,-2) * new Unit(1, BaseUnit.A, -2),
            //  ["Wb"] = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s, -2) * new Unit(1, BaseUnit.A, -1),

            //  ["F"] = new Unit(1, Prefix.k, BaseUnit.g, -1) * new Unit(1, BaseUnit.m, -2) * new Unit(1, BaseUnit.s, 4) * new Unit(1, BaseUnit.A, 2),
            //  ["S"] = new Unit(1, Prefix.k, BaseUnit.g, -1) * new Unit(1, BaseUnit.m, -2) * new Unit(1, BaseUnit.s, 3) * new Unit(1, BaseUnit.A, 2),

            //  ["W"] = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s, -3),
            //  ["J"] = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s, -2),
            //  ["N"] = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m) * new Unit(1, BaseUnit.s, -2),
            
            //  ["Pa"] = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m, -1) * new Unit(1, BaseUnit.s, -2),
            //  ["T"] = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.s, -2) * new Unit(1, BaseUnit.A, -1),
            
            //  ["C"] = new Unit(1, BaseUnit.s) * new Unit(1, BaseUnit.A),
            //  ["Gy"] = new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s, -2),
            //  ["lx"] = new Unit(1, BaseUnit.m, -2) * new Unit(1, BaseUnit.cd),
            //  ["kat"] = new Unit(1, BaseUnit.s, -1) * new Unit(1, BaseUnit.mol),

            //new Unit(60, BaseUnit.s),
        };

        public Unit(double quantity, BaseUnit baseUnit, sbyte power = 1)
        {
            this.Powers = new sbyte[7];
            this.Prefixes = new Prefix[7];
            if(baseUnit > 0)
            {
                this.Powers[(sbyte)baseUnit - 1] = power;
            }
            this.Quantity = quantity;
            this.Symbol = baseUnit.ToString();
        }
        public Unit(double quantity, Prefix prefix, BaseUnit baseUnit, sbyte power = 1)
            : this(quantity, baseUnit, power)
        {
            this.Prefixes[(sbyte)baseUnit - 1] = prefix;

        }
        
        Unit(double quantity, Prefix[] prefixes, sbyte[] powers)
        {
            this.Quantity = quantity;
            this.Powers = powers;
            this.Prefixes = prefixes;
            this.Symbol = powers.ToString();          
        }
        
        /// <summary>
        /// Checks wether a unit has a factor and returns its power. e.g. 'm/s^2' returns -2 with 's' as a parameter. Returns 0 if parameter is not its factor. 
        /// </summary>
        /// <param name="factor">The factor u want to check</param>
        /// <returns>Power of the provided factor</returns>
        public sbyte HasFactor(Unit factor)
        {
            sbyte? power = null;
            for(sbyte i = 0; i < Powers.Length; i++)
            {
                if(factor.Powers[i] != 0)
                {
                    sbyte temp = (sbyte)(Powers[i] / factor.Powers[i]);
                    if(temp == 0)
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
                sbyte reciprocal = (sbyte)(1/power);
                if(Powers.Any(x => x % reciprocal != 0))
                    throw new DimensionSplitException(this, "You can have only integer powers");
            }
            var powers = Powers.Select(x => (sbyte)(x * power)).ToArray();
            return new Unit(Math.Pow(Quantity, power), Prefixes, powers);
        }

        private static IEnumerable<string> FindDerivedUnits(Unit unit, bool fancy, bool divisor)
        {
            var optimal = DerivedUnits
            .Select(derived => new  { Key = derived.Key, Power = unit.HasFactor(derived.Value), Unit = derived.Value })
            .Where(x => x.Power != 0)
            .OrderByDescending(x => x.Unit.Powers.Sum(y => Math.Abs(y)) * Math.Abs(x.Power))
            .FirstOrDefault();
            if(optimal != null)
            {
                Unit remainder = unit / optimal.Unit.Pow(optimal.Power);
                var pow10 = PrefixHelpers.Power10(remainder.Quantity);
                var prfx = PrefixHelpers.FindClosestPrefix(pow10 / optimal.Power);
                var quantity = remainder.Quantity * Math.Pow(10, (int)prfx * optimal.Power);
                yield return Stringifier.UnitToString(prfx, optimal.Key, optimal.Power, fancy);
                var rest = FindDerivedUnits(remainder.NewQuantity(quantity), fancy, divisor);
                foreach(var r in  rest)
                {
                    yield return r;
                }
            }
            else
            {
                foreach(var bs in unit)
                {
                    yield return Stringifier.UnitToString(0, 0.ToString(), 1, fancy);
                }
            }
        }
        
        /// <summary>
        /// Get a single Unit, base or derived, using the string specified
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>new Unit</returns>
        public static Unit GetBySymbol(string symbol)
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
            var copy = new Prefix[7];
            Prefixes.CopyTo(copy, 0);
            copy[(sbyte)baseUnit - 1] = prefix;
            double quantity = Quantity * Math.Pow(10, difference);
            return new Unit(quantity, copy, Powers);
        }
        public Prefix GetPrefix(BaseUnit baseUnit)
        {
            return Prefixes[(sbyte)baseUnit - 1];
        }
        
        public sbyte GetPower(BaseUnit baseUnit)
        {
            return Powers[(sbyte)baseUnit - 1];
        }
        private Unit NewQuantity(double quantity)
        {
            return new Unit(quantity, Prefixes, Powers);
        }
        private int Power10Difference(Unit other)
        {
            int diff = 0;
            for (int i = 0; i < Prefixes.Length; i++)
            {
                if(Powers[i] != 0 && other.Powers[i] != 0)
                {
                    diff += (sbyte)Prefixes[i] * Powers[i] - (sbyte)other.Prefixes[i] * other.Powers[i];
                }
            }
            return diff;
        }
        private Prefix[] PrefixMerge(Unit other, out int diff1, out int diff2, bool? multiplication)
        {
            var result = new Prefix[Prefixes.Length];
            diff1 = 0;
            diff2 = 0;
            for (int i = 0; i < Prefixes.Length; i++)
            {
                //reduction
                if((multiplication == true && Powers[i] + other.Powers[i] == 0) || (multiplication == false && Powers[i] - other.Powers[i] == 0))
                {
                    result[i] = 0;
                    diff2 += ((sbyte)result[i] - (sbyte)other.Prefixes[i]) * other.Powers[i];
                    diff1 += ((sbyte)Prefixes[i] - (sbyte)result[i]) * Powers[i];
                }
                else if(Prefixes[i] != 0)
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
            return result;
        }
        public IEnumerator<Unit> GetEnumerator()
        {
            var units = Enum.GetValues(typeof(BaseUnit));
            foreach (BaseUnit unit in units)
            {
                var factor = this.HasFactor(new Unit(1, unit));
                if(factor != 0)
                {
                    yield return new Unit(1, unit, factor);
                }
            }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
                return Math.Pow(10, power) * Quantity == other.Quantity;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return Quantity.GetHashCode() * Prefixes.GetHashCode() * Powers.GetHashCode();
        }
        public bool IsComparable(Unit other)
        {
            return Powers.SequenceEqual(other.Powers);
        }
        public int CompareTo(Unit other)
        {
            if (IsComparable(other))
            {
                var power = Power10Difference(other);
                return (Math.Pow(10, power) * Quantity).CompareTo(other.Quantity);
            }
            throw new IncomparableUnitsException(this, other, "There you go mixing them again...");
        }
        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            if (!(obj is Unit))
            {
                throw new IncomparableUnitsException(this, obj, "There you go mixing them again...");
            }
            return CompareTo((Unit)obj);
        }

        public override string ToString()
        {
            return ToString("");
        }
        public string ToString(string format)
        {
            format = format.ToLower();
            bool fancy = !format.Contains("c"); // common formmating 
            bool useDivisor = !format.Contains("d"); // use '/'
            bool baseOnly = format.Contains("b"); // base units only
            //string unitString = "";

            //  unitString = char.IsDigit(Symbol, 0) && Symbol[0] != '1' ? 
            //      string.Join(fancy ? Stringifier.Dot : "*", this.SelectMany(x => x).Select(x => x.Symbol)) 
            //      : Symbol;
            //unitString = string.Join("*", FindDerivedUnits(this, true, false));
            //return $"{Quantity}{unitString}";
            var self = this;
            
            var p = Powers.Select((x,i) => self.Powers[i] != 0 ? Stringifier.UnitToString(self.Prefixes[i], ((BaseUnit)(i+1)).ToString(), self.Powers[i], fancy) : "").Where(x => x != "");
            return $"{Quantity}{string.Join(Stringifier.Dot, p)}";
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
            throw new IncomparableUnitsException(u1, u2, "You can't mix them. You just can't");
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
            throw new IncomparableUnitsException(u1, u2, "You can't mix them. You just can't");
        }

        public static Unit operator *(Unit u1, Unit u2)
        {
            var p1 = 0;
            var p2 = 0;
            var prefixes = u1.PrefixMerge(u2, out p1, out p2, true);
            var powers = u1.Powers.Zip(u2.Powers, (x,y) => (sbyte)(x + y)).ToArray();
            var quantity = u1.Quantity * u2.Quantity * Math.Pow(10, p1) * Math.Pow(10, p2);
            return new Unit(quantity, prefixes, powers);
        }

        public static Unit operator /(Unit u1, Unit u2)
        {
            var p1 = 0;
            var p2 = 0;
            var prefixes = u1.PrefixMerge(u2, out p1, out p2, false);
            var powers = u1.Powers.Zip(u2.Powers, (x,y) => (sbyte)(x - y)).ToArray();
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
