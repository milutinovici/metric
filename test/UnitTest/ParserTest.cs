using MeasurementUnits;
using System;
using System.Linq;
using Xunit;

namespace UnitTest
{
    public class ParserTest
    {

        [Fact]
        public void ParseBaseUnit()
        {
            var u1 = new Unit(12, BaseUnit.g);
            var u2 = Unit.Parse("12g");
            Assert.Equal(u1, u2);
        }

        [Fact]
        public void ParsePrefixedBaseUnit()
        {
            var u1 = new Unit(12, Prefix.n, BaseUnit.m);
            var u2 = Unit.Parse("12nm");
            Assert.Equal(u1, u2);
        }

        [Fact]
        public void ParsePoweredPrefixedBaseUnit()
        {
            var u1 = new Unit(12, Prefix.n, BaseUnit.m, -4);
            var u2 = Unit.Parse("12nm^-4");
            Assert.Equal(u1, u2);
        }

        [Fact]
        public void ParseDerivedUnitPoweredPrefixed()
        {
            string s = "1mV^-3";
            var u = Unit.Parse(s).Pow(-1);
            Assert.Equal("1mV^3", u.ToString("c"));
        }

        [Fact]
        public void ParseDerivedUnitPrefixed()
        {
            string s = "13mF";
            var u = Unit.Parse(s);
            Assert.Equal("13mF", u.ToString());
        }

        [Fact]
        public void ParseNonexistantUnit()
        {
            string s = "13mZ^2";
            Assert.Throws<FormatException>(() => Unit.Parse(s));
        }
    }
}
