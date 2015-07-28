using MeasurementUnits;
using System.Linq;
using Xunit;

namespace UnitTest
{
    public class ComplexUnitTest
    {
        public Unit N { get; set; }
        public Unit S { get; set; }


        public ComplexUnitTest()
        {
            N = new Unit(1, BaseUnit.m) * new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.s, -2);
            S = new Unit(1, BaseUnit.s);
        }

        [Fact]
        public void IsComparableComplexWithDifferentPrefixes()
        {
            var kn = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, Prefix.k, BaseUnit.m) * new Unit(1, BaseUnit.s, -2);
            Assert.True(N.IsComparable(kn));
        }

        [Fact]
        public void MultiplicationOfComplexAndBaseUnit()
        {
            var u = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m) * new Unit(1, BaseUnit.s, -1);
            Assert.Equal(u, (N * S));
        }
        [Fact]
        public void ReductionOfUnitWithPrefix()
        {
            var u1 = new Unit(1, Prefix.k, BaseUnit.m) * new Unit(1, BaseUnit.s, -1);
            var u2 = new Unit(1, Prefix.m, BaseUnit.s, 2) * new Unit(1, BaseUnit.m, -1);
            var r = u1 * u2;
            Assert.Equal(r, new Unit(1, Prefix.m, BaseUnit.s));
        }
        
        [Fact]
        public void HasFactor()
        {
            var u1 = N.Pow(-1) / S;
            var u2 = N.Pow(-1) * S;
            Assert.Equal(-1, N.HasFactor(u1));
            Assert.Equal(0, N.HasFactor(u2));
        }

        [Fact]
        public void HasFactorBaseUnit()
        {
            int power = N.HasFactor(S);
            Assert.Equal(-2, power);
        }

        [Fact]
        public void HasFactorComplexUnit()
        {

            int power = N.Pow(2).HasFactor(N);
            Assert.Equal(2, power);
        }

        [Fact]
        public void HasFactorNegativeComplexUnit()
        {
            int power = N.HasFactor(N.Pow(-1));
            Assert.Equal(-1, power);
        }

        [Fact]
        public void MultiplyDerivedUnitsWithPrefix()
        {
            var kn = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, Prefix.k, BaseUnit.m) * new Unit(1, BaseUnit.s, -2);
            var n = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m) * new Unit(1, BaseUnit.s, -2);
            var c = n * kn;
            Assert.Equal(0.001, c.Quantity);
            Assert.Equal(Prefix.k, c.GetPrefix(BaseUnit.g));
        }
        
        [Fact]
        public void kWEquals1000W()
        {
            var w = new Unit(1000, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s, -3);
            var kw = new Unit(1, Prefix.M, BaseUnit.g) * new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s, -3);
            Assert.Equal(w, kw);
        }

        [Fact]
        public void ChangePrefixDerived()
        {
            var mv = Unit.Create("V") / 1000;
            Assert.Equal("1mV", mv.ToString());
        }

        [Fact]
        public void RecognizeDerivedUnit()
        {
            var mn = new Unit(12, BaseUnit.m) * new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.s, -2);
            Assert.Equal("12N", mn.ToString()); 
        }

        [Fact]
        public void RecognizeDerivedUnitWithPrefix()
        {
            var mn = new Unit(5, BaseUnit.g) * new Unit(1, BaseUnit.m) * new Unit(1, BaseUnit.s, -2);
            Assert.Equal("5mN", mn.ToString());
        }
        
        [Fact]
        public void BaseAndDerivedUnit()
        {
            Unit c = (N / S);
            Assert.Equal("1N" + SingleUnit.Dot + "s" + SingleUnit.SS(-1), c.ToString());
        }

        [Fact]
        public void RecognizeDerivedUnitWithPrefix2()
        {
            var u = new Unit(2, Prefix.h, BaseUnit.g) * new Unit(1, Prefix.M, BaseUnit.m) * new Unit(1, BaseUnit.s, -2);
            Assert.Equal("0.2MN", u.ToString());
        }

        [Fact]
        public void WithDivisor()
        {
            var u = new Unit(1, Prefix.m, BaseUnit.m) * new Unit(1, BaseUnit.s, -2);
            Assert.Equal("1mm/s^2", u.ToString("dc"));
        }

        [Fact]
        public void CreateDerivedUnitPrefixed()
        {
            var u = Unit.Create(Prefix.m, "F");
            Assert.Equal("1mF", u.ToString());
        }

        [Fact]
        public void CreateDerivedUnitPrefixed2()
        {
            var u = Unit.Create(Prefix.m, "Gy");
            Assert.Equal("1mGy", u.ToString());
        }

    }
}

