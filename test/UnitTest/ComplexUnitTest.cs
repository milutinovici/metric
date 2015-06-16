using System.Linq;
using MeasurementUnits;
using Xunit;

namespace UnitTest
{
    public class ComplexUnitTest
    {
        public Unit N { get; set; }
        public Unit S { get; set; }


        public ComplexUnitTest()
        {
            N = new Unit(new Unit(BaseUnit.m), new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.s, -2));
            S = new Unit(BaseUnit.s);
        }

        [Fact]
        public void IsAddableComplexWithDifferentPrefixes()
        {
            var kn = new Unit(new Unit(Prefix.k, BaseUnit.g), new Unit(Prefix.k, BaseUnit.m), new Unit(BaseUnit.s, -2));
            Assert.True(N.IsComparable(kn));
        }

        [Fact]
        public void MultiplicationOfComplexAndBaseUnit()
        {
            Assert.Equal("1kg" + Stringifier.Dot + "m" + Stringifier.Dot + "s" + Stringifier.SS(-1), (N * S).ToString());
        }
        [Fact]
        public void DivisionOfComplexAndBaseUnit()
        {
            Assert.Equal("1N" + Stringifier.Dot + "s" + Stringifier.SS(-1), (N / S).ToString());
        }

        [Fact]
        public void DivisionOf2ComplexUnits()
        {
            var u = new Unit(new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m), new Unit(BaseUnit.s, -1));
            var u1 = N / u;
            Assert.Equal("1s" + Stringifier.SS(-1), u1.ToString());
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
        public void RecognizeDerivedUnit()
        {
            Assert.Equal("1N", N.ToString());
        }

        [Fact]
        public void RecognizeDerivedUnitWithPrefix()
        {
            var kn = new Unit(1, new Unit(Prefix.k, BaseUnit.g), new Unit(Prefix.k, BaseUnit.m), new Unit(BaseUnit.s, -2));
            Assert.Equal("1kN", kn.ToString());
        }

        [Fact]
        public void MultiplyDerivedUnits()
        {
            Unit c = N * N;
            Assert.Equal("1N" + Stringifier.SS(2), c.ToString());
        }

        [Fact]
        public void MultiplyDerivedUnitsWithPrefix()
        {
            var kn = new Unit(new Unit(Prefix.k, BaseUnit.g), new Unit(Prefix.k, BaseUnit.m), new Unit(BaseUnit.s, -2));
            var n = new Unit(new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m), new Unit(BaseUnit.s, -2));
            var c = n * kn;
            Assert.Equal("10daN" + Stringifier.SS(2), c.ToString());
        }

        [Fact]
        public void ComplexDerivedUnit()
        {
            Unit c = (N / S);
            Assert.Equal("1N" + Stringifier.Dot + "s" + Stringifier.SS(-1), c.ToString());
        }

        [Fact]
        public void PrefixTransferDown()
        {
            Unit n = new Unit(new Unit(BaseUnit.m), new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.s, -2));
            n = n.ChangePrefix(Prefix.m);
            var u = n.Units.First();
            Assert.Equal(Prefix.m, n.Prefix);
        }

        [Fact]
        public void ParseDerivedUnitPoweredPrefixed()
        {
            string s = "1mV^-3";
            var u = Unit.Parse(s).Pow(-1);
            Assert.Equal("1mV^3", u.ToString("c"));
        }

        [Fact]
        public void RecognizeDerivedUnitWithPrefix2()
        {
            var u = new Unit(2, new Unit(Prefix.h, BaseUnit.g), new Unit(Prefix.M, BaseUnit.m), new Unit(BaseUnit.s, -2));
            Assert.Equal("200kN", u.ToString());
        }
        [Fact]
        public void kWEquals1000W()
        {
            var w = new Unit(1000, new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -3));
            var kw = new Unit(1, new Unit(Prefix.M, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -3));
            Assert.True(w.Equals(kw));
        }
        [Fact]
        public void HasFactor()
        {
            int f1 = N.HasFactor(N.Pow(-1) / S);
            int f2 = N.HasFactor(N.Pow(-1) * S);
            Assert.Equal(-1, f1);
            Assert.Equal(0, f2);
        }
        [Fact]
        public void SquareRoot()
        {
            var u = N.Pow(2).Pow(0.5);
 
            Assert.Equal("1N", u.ToString());
        }
        [Fact]
        public void DimensionSplit()
        {
            Assert.Throws<DimensionSplitException>(() => N.Pow(0.5));
        }
    }
}

