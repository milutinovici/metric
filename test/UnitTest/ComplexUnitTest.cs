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
        public void ComplexEnumeratesToBaseUnits()
        {
            var w = new Unit(20, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s, -3);
            var result = w.ToArray();
            for (int i = 0; i < result.Length; i++)
            {
                System.Console.WriteLine(result[i]);
            }
            Assert.Equal(result.Length, 3);
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
            Assert.Equal("1N" + Stringifier.Dot + "s" + Stringifier.SS(-1), c.ToString());
        }

        //  [Fact]
        //  public void ParseDerivedUnitPoweredPrefixed()
        //  {
        //      string s = "1mV^-3";
        //      var u = Unit.Parse(s).Pow(-1);
        //      Assert.Equal("1mV^3", u.ToString("c"));
        //  }

        //  [Fact]
        //  public void RecognizeDerivedUnitWithPrefix2()
        //  {
        //      var u = new Unit(2, Prefix.h, BaseUnit.g) * new Unit(1, Prefix.M, BaseUnit.m) * new Unit(1, BaseUnit.s, -2);
        //      Assert.Equal("200kN", u.ToString());
        //  }



    }
}

