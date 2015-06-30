using System;
using System.Collections.Generic;
using System.Linq;


namespace MeasurementUnits
{
    public struct Unit : IEnumerable<Unit>, IEquatable<Unit>, IComparable<Unit>, IComparable
    {
        BaseUnit Numerator { get; } 
        BaseUnit Denominator { get;}
        BaseUnit PositivePrefix { get; }
        BaseUnit NegativePrefix { get; } 
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

        public Unit(double quantity, BaseUnit baseUnit, int power = 1)
        {
            if(power > 0)
            {
                this.Numerator = baseUnit;
                this.Denominator = (BaseUnit)1;
            }
            else
            {
                this.Numerator = (BaseUnit)1;
                this.Denominator = baseUnit;
            }


            this.PositivePrefix = (BaseUnit)1;
            this.NegativePrefix = (BaseUnit)1;
            this.Quantity = quantity;
            this.Symbol = baseUnit.ToString();
        }
        public Unit(double quantity, Prefix prefix, BaseUnit baseUnit, int power = 1)
            : this(quantity, baseUnit, power)
        {
            if(prefix < 0) {
                this.NegativePrefix = baseUnit.Pow((uint)prefix);
            }
            else {   
                this.PositivePrefix = baseUnit.Pow((uint)prefix);
            }    
            
        }
        
        Unit(double quantity, BaseUnit numerator, BaseUnit denominator, BaseUnit positive, BaseUnit negative)
        {
            this.Quantity = quantity;

            var simplified = BaseHelpers.Reduce(numerator, denominator);
            this.Numerator = simplified.Item1;
            this.Denominator = simplified.Item2;
            
            simplified = BaseHelpers.Reduce(positive, negative); 
            this.PositivePrefix = simplified.Item1;
            this.NegativePrefix = simplified.Item2;
            
            this.Symbol = numerator.ToString();            
        }
        
        /// <summary>
        /// Checks wether a unit has a factor and returns its power. e.g. 'm/s^2' returns -2 with 's' as a parameter. Returns 0 if parameter is not its factor. 
        /// </summary>
        /// <param name="factor">The factor u want to check</param>
        /// <returns>Power of the provided factor</returns>
        public int HasFactor(Unit factor)
        {
            var p = Math.Min(Numerator.HasFactor(factor.Numerator), Denominator.HasFactor(factor.Denominator));
            var n = Math.Min(Numerator.HasFactor(factor.Denominator), Denominator.HasFactor(factor.Numerator));
            //  Console.WriteLine(Numerator);
            //  Console.WriteLine(factor.Numerator);
            return p != 0 ? p : n;
        }
        /// <summary>
        /// Returns a new unit with the specified power. 
        /// </summary>
        /// <param name="power">power</param>
        /// <exception>You can have only int powers. You cannot power m^3 with 0.5, because m^1.5 doesn't make sense
        ///   <cref>DimensionSplitException</cref>
        /// </exception>
        /// <returns>new Unit</returns>
        public Unit Pow(double power)
        {
            if(power < 0)
            {
                return new Unit(Math.Pow(Quantity, power), 
                            Denominator.Pow((uint)power),
                            Numerator.Pow((uint)power), 
                            PositivePrefix, 
                            NegativePrefix);
            }
            else
            {
                return new Unit(Math.Pow(Quantity, power), 
                            Numerator.Pow((uint)power), 
                            Denominator.Pow((uint)power), 
                            PositivePrefix, 
                            NegativePrefix);
            }

        }
        public int GetPrefix(BaseUnit unit)
        {
            var log = Math.Log((ulong)PositivePrefix, (uint)unit);
            if(log % 1 == 0)
                return (int)log;
            log = Math.Log((ulong)NegativePrefix, (uint)unit);
            if(log % 1 == 0)
                return (int)-log;
            return 0;
        }

        public Prefix GetAggregatePrefix()
        {
            if(PositivePrefix != 0 || NegativePrefix != 0)
            {
                return (Prefix)Enum.GetValues(typeof(BaseUnit))
                                   .Cast<BaseUnit>()
                                   .Select(GetPrefix)
                                   .Sum(x => (int)x);
            }
            else return 0;
        }
        
        private static IEnumerable<string> FindDerivedUnits(Unit unit, bool fancy, bool divisor)
        {
            var optimal = DerivedUnits
            .Select(derived => new  { Key = derived.Key, Power = unit.HasFactor(derived.Value), Unit = derived.Value })
            .Where(x => x.Power != 0)
            .OrderByDescending(x => x.Unit.Sum(y => Math.Abs((decimal)y.Numerator * (ulong)y.Denominator)) * Math.Abs(x.Power))
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
                    yield return Stringifier.UnitToString(bs.GetAggregatePrefix(), bs.Numerator.ToString(), 1, fancy);
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
        public Unit ChangePrefix(Prefix prefix)
        {
            int difference = (prefix - GetAggregatePrefix()) * 1;
            double quantity = Quantity;
            return new Unit(quantity, prefix, Numerator, 1);
        }
        private Unit NewQuantity(double quantity)
        {
            return new Unit(quantity, Numerator, Denominator, PositivePrefix, NegativePrefix);
        }
        //  private int Power10Difference(Prefix prefix)
        //  {
        //      return Power * ((int)Prefix - (int)prefix);
        //  }
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
                var power1 = (int)this.GetAggregatePrefix();
                var power2 = (int)other.GetAggregatePrefix();
                return Math.Pow(10, power1) * Quantity == Math.Pow(10, power2) * other.Quantity;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return (int)Quantity * Numerator.GetHashCode() 
                                 / Denominator.GetHashCode() 
                                 * PositivePrefix.GetHashCode() 
                                 / NegativePrefix.GetHashCode();
        }
        public bool IsComparable(Unit other)
        {
            return Numerator == other.Numerator && Denominator == other.Denominator;
        }
        public int CompareTo(Unit other)
        {
            if (IsComparable(other))
            {
                var power1 = (int)GetAggregatePrefix();
                var power2 = (int)other.GetAggregatePrefix();
                return (Math.Pow(10, power1) * Quantity).CompareTo(Math.Pow(10, power2) * other.Quantity);
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
            string quantity = !format.Contains("i") ? Quantity.ToString() : ""; // ignore quantity
            bool fancy = !format.Contains("c"); // common formmating 
            bool useDivisor = !format.Contains("d"); // use '/'
            bool baseOnly = format.Contains("b"); // base units only
            string unitString = "";

            unitString = char.IsDigit(Symbol, 0) && Symbol[0] != '1' ? 
                string.Join(fancy ? Stringifier.Dot : "*", this.SelectMany(x => x).Select(x => x.Symbol)) 
                : Symbol;

            return string.Format("{0}{1}", quantity, unitString);
        }
        #region Operators
        public static Unit operator +(Unit u1)
        {
            return u1;
        }
        public static Unit operator -(Unit u1)
        {
            return u1.NewQuantity(u1.Quantity * (-1));
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
                var averagePrefix = PrefixHelpers.AveragePrefix(u1.GetAggregatePrefix(), u2.GetAggregatePrefix());
                var quantity = u1.Quantity*Math.Pow(10, 0) +
                               u2.Quantity*Math.Pow(10, 0);
                var newUnit = new Unit(quantity, averagePrefix, u1.Numerator, 1);
                return newUnit;
            }
            throw new IncomparableUnitsException(u1, u2, "You can't mix them. You just can't");
        }

        public static Unit operator -(Unit u1, Unit u2)
        {
            if (u1.IsComparable(u2))
            {
                var averagePrefix = PrefixHelpers.AveragePrefix(u1.GetAggregatePrefix(), u2.GetAggregatePrefix());
                var quantity = u1.Quantity*Math.Pow(10, 0) -
                               u2.Quantity*Math.Pow(10, 0);
                var newUnit = new Unit(quantity, averagePrefix, u1.Numerator, 1);
                return newUnit;
            }
            throw new IncomparableUnitsException(u1, u2, "You can't mix them. You just can't");
        }

        public static Unit operator *(Unit u1, Unit u2)
        {
            return new Unit(u1.Quantity * u2.Quantity, 
                            (BaseUnit)((uint)u1.Numerator * (uint)u2.Numerator), 
                            (BaseUnit)((uint)u1.Denominator * (uint)u2.Denominator), 
                            u1.PositivePrefix.AverageFactors(u2.PositivePrefix),
                            u1.NegativePrefix.AverageFactors(u2.NegativePrefix));

        }

        public static Unit operator /(Unit u1, Unit u2)
        {
            var numerator = (uint)u2.Numerator != 0 ? (BaseUnit)((uint)u1.Numerator / (uint)u2.Numerator) : u1.Numerator;
            var denominator = (uint)u2.Denominator != 0 ? (BaseUnit)((uint)u1.Denominator / (uint)u2.Denominator) : u1.Denominator;
            return new Unit(u1.Quantity / u2.Quantity, 
                            numerator, 
                            denominator, 
                            u1.PositivePrefix.AverageFactors(u2.PositivePrefix),
                            u1.NegativePrefix.AverageFactors(u2.NegativePrefix));
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
