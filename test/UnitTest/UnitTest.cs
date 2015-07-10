using MeasurementUnits;
using Xunit;

namespace UnitTest
{
    public class UnitTest
    {
        public Unit M { get; set; }
        public Unit Kg { get; set; }
        public Unit S { get; set; }
        public Unit Sm2 { get; set; }

        public UnitTest()
        {
            M = new Unit(1, BaseUnit.m);
            Kg = new Unit(1, Prefix.k, BaseUnit.g, 1);
            S = new Unit(1, BaseUnit.s);
            Sm2 = new Unit(1, BaseUnit.s, -2);
        }

        [Fact]
        public void HasFactor()
        {
            Assert.Equal(-2, Sm2.HasFactor(S));
        }

        [Fact]
        public void Equality()
        {
            var m1 = new Unit(1, BaseUnit.m);
            var m2 = new Unit(1, BaseUnit.m);
            Assert.Equal(m1, m2);
        }
        [Fact]
        public void CompareTo()
        {
            var m2 = new Unit(2, BaseUnit.m);
            var m1 = new Unit(1, BaseUnit.m);
            Assert.Equal(1, m2.CompareTo(m1));
        }
        [Fact]
        public void CompareToPrefix()
        {
            var m = new Unit(999, BaseUnit.m);
            var km = new Unit(1, Prefix.k, BaseUnit.m);
            Assert.Equal(-1, m.CompareTo(km));
        }

        [Fact]
        public void Addition()
        {
            var m2 = new Unit(2, BaseUnit.m);
            Assert.Equal(m2.Quantity, (M + M).Quantity);
        }

        [Fact]
        public void AdditionOfIncomparableUnits()
        {
            Assert.Throws<IncomparableUnitsException>(() => Kg + M);
        }

        [Fact]
        public void SubstractionWithPrefix()
        {
            var k1= new Unit(0, BaseUnit.g);
            Assert.Equal(k1, (Kg - Kg));
        }

        [Fact]
        public void Powerment()
        {
            var m2 = new Unit(1, BaseUnit.m, -2);
            Assert.Equal(m2, M.Pow(-2));
        }

        [Fact]
        public void SelfMultiplication()
        {
            var m2 = new Unit(1, BaseUnit.m, 2);
            Assert.Equal(m2, M * M);
        }

        [Fact]
        public void SelfDivision()
        {
            var one = new Unit(1, (BaseUnit)1);
            Assert.Equal(one, (M / M));
        }
        
        [Fact]
        public void Division()
        {
            var s2 = new Unit(1, BaseUnit.s, 2);
            var sm1 = new Unit(1, BaseUnit.s, -1);
            Assert.Equal(sm1.ToString(), (S / s2).ToString());
        }

        [Fact]
        public void GMultiplication()
        {
            var ms = new Unit(1, (BaseUnit)((ulong)BaseUnit.m * (ulong)BaseUnit.s));
            Assert.Equal(ms, (M * S));
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
            var km = m.ChangePrefix(Prefix.k);
            System.Console.WriteLine(km);
            Assert.Equal(m, km);
            Assert.Equal(m.Quantity, km.Quantity * 1000);
        }

        [Fact]
        public void NumberAddition()
        {
            var nw = S + 100;
            Assert.Equal(nw.Quantity, 101);
        }

        [Fact]
        public void NumberSubstraction()
        {
            var nw = S - 100;
            Assert.Equal(nw.Quantity, -99);
        }

        [Fact]
        public void NumberMultiplication()
        {
            var nw = M * 100;
            Assert.Equal(nw.Quantity, 100);
        }

        [Fact]
        public void NumberDivision()
        {
            var mm = new Unit(100, BaseUnit.m, -1);
            var nw =  100 / M;
            Assert.Equal(mm, nw);
        }
        
        [Fact]
        public void SquareRoot()
        {
            var m2 = new Unit(1, BaseUnit.m, 2);
            var u = m2.Pow(0.5);
            Assert.Equal(M, u);
        }
        [Fact]
        public void DimensionSplit()
        {
            Assert.Throws<DimensionSplitException>(() => M.Pow(0.5));
        }
        
        //  [Fact]
        //  public void PrefixReduction()
        //  {
        //      var u1 = new Unit(1, Prefix.k, BaseUnit.m) / new Unit(1, Prefix.k, BaseUnit.s);

        //      Assert.Equal(u1.Prefix, (Prefix)0);
        //  }

    }
}
