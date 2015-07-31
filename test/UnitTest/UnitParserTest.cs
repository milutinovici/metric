using Metric;
using System;
using Xunit;

namespace UnitTest
{
    public class UnitParserTest
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
        public void ParseBaseUnitPowered()
        {
            string s = "1m^2";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString("c"));
        }

        [Fact]
        public void ParseBaseUnitPrefixedPowered()
        {
            string s = "1km^3";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString("c"));
        }

        [Fact]
        public void ParseNonexistantUnit()
        {
            string s = "13mZ^2";
            Assert.Throws<FormatException>(() => Unit.Parse(s));
        }

        [Fact]
        public void ParseComplexUnit()
        {
            string s = "1m*s";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString("c"));
        }

        [Fact]
        public void ParseComplexUnitDivided()
        {
            string s = "1m/s";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString("d"));
        }

        [Fact]
        public void ParseComplexUnitPowered()
        {
            string s = "1m^2*s";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString("c"));
        }

        [Fact]
        public void ParseComplexUnitPoweredPrefixed()
        {
            string s = "1km^2*s^-1";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString("c"));
        }

        [Fact]
        public void ParseComplexUnitDividedPoweredPrefixed()
        {
            string s = "1m^3/s^2";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString("cdb"));
        }

        [Fact]
        public void ParseDerivedUnit()
        {
            string s = "1V";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString());
        }

        [Fact]
        public void ParseDerivedUnitPowered()
        {
            string s = "1V^2";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString("c"));
        }

        [Fact]
        public void ParseDerivedUnitPrefixed1()
        {
            string s = "1mV";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString());
        }

        [Fact]
        public void ParseDerivedUnitPoweredPrefixed()
        {
            string s = "1mV^-3";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString("c"));
        }

        [Fact]
        public void ParseDerivedComplexUnitPoweredPrefixed()
        {
            string s = "1mV^-2*K";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString("c"));
        }

        [Fact]
        public void ParseDerivedLength2()
        {
            string s = "1Pa";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString("c"));
        }

        [Fact]
        public void ParseDerivedLength3PrefixedPowered()
        {
            string s = "1mkat^2";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString("c"));
        }

        [Fact]
        public void ParseMeasurementPrefixedPowered()
        {
            string s = 16.25 + "MT^2";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString("c"));
        }

        [Fact]
        public void ParseMixed()
        {
            string s = "12MT^-2*kmol^-1";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString("c"));
        }

    }
}
