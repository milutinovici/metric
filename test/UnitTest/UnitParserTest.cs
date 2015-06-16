using MeasurementUnits;
using Xunit;

namespace UnitTest
{
    public class UnitParserTest
    {
        [Fact]
        public void ParseBaseUnit()
        {
            string s = "1m";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString());
        }

        [Fact]
        public void ParseBaseUnitPowered()
        {
            string s = "1m^2";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString("c"));
        }

        [Fact]
        public void ParseBaseUnitPrefixed()
        {
            string s = "1km";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString());
        }

        [Fact]
        public void ParseBaseUnitPrefixedPowered()
        {
            string s = "1km^3";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString("c"));
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
        public void ParseDerivedUnitPrefixed()
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
            string s = "1K*mV^-2";
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
        public void ConvertSuperscript()
        {
            string s = "MT" + Stringifier.SS(2);
            var normal = UnitParser.ConvertSuperscript(s);
            Assert.Equal("MT^2", normal);
        }

        [Fact]
        public void ParseMixed()
        {
            string s = "12kmol^-1*MT^-2";
            var u = Unit.Parse(s);
            Assert.Equal(s, u.ToString("c"));
        }
 
    }
}
