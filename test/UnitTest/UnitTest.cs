using Metric;
using Xunit;

namespace UnitTest
{
    public class UnitTest
    {
        [Fact]
        public void HasNegativeFactor()
        {
            var u1 = new Unit(2, BaseUnit.s, -2);
            var u2 = new Unit(1, BaseUnit.s);
            Assert.Equal(-2, u1.HasFactor(u2));
        }

        [Fact]
        public void EqualityBaseUnit()
        {
            var u1 = new Unit(1, BaseUnit.m);
            var u2 = new Unit(1, BaseUnit.m);
            Assert.Equal(u1, u2);
        }
        [Fact]
        public void CompareToBaseUnit()
        {
            var u1 = new Unit(2, BaseUnit.m);
            var u2 = new Unit(1, BaseUnit.m);
            Assert.Equal(1, u1.CompareTo(u2));
        }
        [Fact]
        public void CompareToWithPrefix()
        {
            var m = new Unit(999, BaseUnit.m);
            var km = new Unit(1, Prefix.k, BaseUnit.m);
            Assert.Equal(-1, m.CompareTo(km));
        }

        [Fact]
        public void Addition()
        {
            var u1 = new Unit(1, BaseUnit.m);
            var u2 = new Unit(2, BaseUnit.m);
            Assert.Equal(u2.Quantity, (u1 + u1).Quantity);
        }

        [Fact]
        public void AdditionOfIncomparableUnits()
        {
            var u1 = new Unit(1, BaseUnit.g);
            var u2 = new Unit(2, BaseUnit.mol);
            Assert.Throws<IncomparableUnitsException>(() => u1 + u2);
        }

        [Fact]
        public void SelfSubstractionWithPrefix()
        {
            var u1 = new Unit(3, Prefix.k, BaseUnit.g);
            var u2 = new Unit(0, BaseUnit.g);
            Assert.Equal(u2, (u1 - u1));
        }
        
        [Fact]
        public void DivisionWithPrefix()
        {
            var u1 = new Unit(4, Prefix.m, BaseUnit.m, 2);
            var u2 = new Unit(2, Prefix.n, BaseUnit.m);
            var r = u1/u2;
            Assert.Equal(r, new Unit(2, Prefix.k, BaseUnit.m));
        }
        
        [Fact]
        public void Powerment()
        {
            var u1 = new Unit(1, BaseUnit.m);
            var u2 = new Unit(1, BaseUnit.m, -2);
            Assert.Equal(u2, u1.Pow(-2));
        }

        [Fact]
        public void SelfMultiplication()
        {
            var u1 = new Unit(2, BaseUnit.m);
            var u2 = new Unit(4, BaseUnit.m, 2);
            Assert.Equal(u2, u1 * u1);
        }

        [Fact]
        public void SelfDivision()
        {
            var u1 = new Unit(4, BaseUnit.m);
            var one = new Unit(1, 0);
            Assert.Equal(one, (u1 / u1));
        }
        
        [Fact]
        public void Division()
        {
            var u1 = new Unit(1, BaseUnit.s);
            var u2 = new Unit(1, BaseUnit.s, 2);
            var sm1 = new Unit(1, BaseUnit.s, -1);
            Assert.Equal(sm1, (u1 / u2));
        }

        [Fact]
        public void FindClosestPrefix()
        {
            var prfx = PrefixHelpers.FindClosestPrefix(-59);
            Assert.Equal(Prefix.y, prfx);
        }

        [Fact]
        public void ChangePrefixBase()
        {
            var m = new Unit(1, BaseUnit.m);
            var km = m.ChangePrefix(Prefix.k, BaseUnit.m);
            Assert.Equal(m, km);
            Assert.Equal(m.Quantity, km.Quantity * 1000);
        }

        [Fact]
        public void NumberAddition()
        {
            var u1 = new Unit(1, BaseUnit.K);
            var u2 = u1 + 100;
            Assert.Equal(u2.Quantity, 101);
        }

        [Fact]
        public void NumberSubstraction()
        {
            var u1 = new Unit(1, BaseUnit.K);
            var u2 = u1 - 100;
            Assert.Equal(u2.Quantity, -99);
        }

        [Fact]
        public void NumberMultiplication()
        {
            var u1 = new Unit(5, BaseUnit.K);
            var u2 = u1 * 4;
            Assert.Equal(u2.Quantity, 20);
        }

        [Fact]
        public void NumberDivision()
        {
            var u1 = new Unit(1, BaseUnit.m);
            var u2 =  100 / u1;
            Assert.Equal(new Unit(100, BaseUnit.m, -1), u2);
        }
        
        [Fact]
        public void SquareRoot()
        {
            var u1 = new Unit(9, BaseUnit.m, 2);
            var result = u1.Pow(0.5);
            Assert.Equal(new Unit(3, BaseUnit.m), result);
        }
        [Fact]
        public void DimensionSplit()
        {
            var u1 = new Unit(1, BaseUnit.m);
            Assert.Throws<DimensionSplitException>(() => u1.Pow(0.5));
        }
        
        [Fact]
        public void PrefixReduction()
        {
            var u1 = new Unit(10, Prefix.k, BaseUnit.m) / new Unit(2, Prefix.k, BaseUnit.m);
            Assert.Equal(u1.GetPrefix(BaseUnit.m), (Prefix)0);
        }

        [Fact]
        public void PrefixReduction1()
        {
            var u1 = new Unit(10, Prefix.k, BaseUnit.m) / new Unit(2, Prefix.k, BaseUnit.m);
            Assert.Equal(u1.GetPrefix(BaseUnit.m), (Prefix)0);
        }

        [Fact]
        public void PrefixedToString()
        {
            var u1 = new Unit(1, Prefix.k, BaseUnit.mol);
            Assert.Equal("1kmol", u1.ToString());
        }

    }
}
