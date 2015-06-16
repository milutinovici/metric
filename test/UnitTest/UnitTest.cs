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
            M = new Unit(BaseUnit.m);
            Kg = new Unit(Prefix.k, BaseUnit.g, 1);
            S = new Unit(BaseUnit.s);
            Sm2 = new Unit(BaseUnit.s, -2);
        }

        [Fact]
        public void TestToString()
        {
            Assert.Equal("1m", M.ToString());
        }

        [Fact]
        public void ToStringWithPrefix()
        {
            Assert.Equal("1kg", Kg.ToString());
        }

        [Fact]
        public void ToStringWithPrefixAndPower()
        {
            Assert.Equal("1kg" + Stringifier.SS(2), Kg.Pow(2).ToString());
        }

        [Fact]
        public void Addition()
        {
            Assert.Equal("2m", (M + M).ToString());
        }

        [Fact]
        public void AdditionOfGrandmothersAndFrogs()
        {
            Assert.Throws<GrandmothersAndFrogsException>(() => Kg + M);
        }

        [Fact]
        public void SubstractionWithPrefix()
        {
            Assert.Equal("0kg", (Kg - Kg).ToString());
        }

        [Fact]
        public void Powerment()
        {
            Assert.Equal("1m" + Stringifier.SS(2), M.Pow(2).ToString());
        }

        [Fact]
        public void SelfMultiplication()
        {
            Assert.Equal("1m" + Stringifier.SS(2), (M * M).ToString());
        }

        [Fact]
        public void SelfDivision()
        {
            Assert.Equal("1", (M / M).ToString());
        }

        [Fact]
        public void GMultiplication()
        {
            Assert.Equal("1m" + Stringifier.Dot + "s", (M * S).ToString());
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
            var m = new Unit(BaseUnit.m);
            var km = m.ChangePrefix(Prefix.k);
            Assert.Equal("1km", km.ToString());
        }
        [Fact]
        public void ChangePrefixDerived()
        {  
            var n = Unit.GetBySymbol("N");
            var mN = n.ChangePrefix(Prefix.m);
            Assert.Equal("1mN", mN.ToString());
        }

        [Fact]
        public void NumberAddition()
        {
            var w = Unit.GetBySymbol("W");
            var nw = w + 100;
            Assert.Equal("101W", nw.ToString());
        }

        [Fact]
        public void NumberSubstraction()
        {
            var w = Unit.GetBySymbol("W");
            var nw = w - 100;
            Assert.Equal("-99W", nw.ToString());
        }

        [Fact]
        public void NumberMultiplication()
        {
            var w = Unit.GetBySymbol("W");
            var nw = w * 100;
            Assert.Equal("100W", nw.ToString());
        }

        [Fact]
        public void NumberDivision()
        {
            var w = Unit.GetBySymbol("W");
            var nw =  100 / w;
            Assert.Equal("100W^-1", nw.ToString("c"));
        }

        [Fact]
        public void ChooseRemainWithSmallestPower()
        {
            var w = Unit.GetBySymbol("W");
            var nw = w / S;
            Assert.Equal("1W/s", nw.ToString("d"));
        }

        [Fact]
        public void CompareTo()
        {
            var m = new Unit(999, BaseUnit.m);
            var km = new Unit(1, Prefix.k, BaseUnit.m);
            Assert.Equal(-1, m.CompareTo(km));
        }
    }
}
