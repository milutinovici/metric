using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeasurementUnits;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class UnitParserTest
    {
        [TestMethod]
        public void ParseBaseUnit()
        {
            string s = "1m";
            var u = Unit.Parse(s);
            Assert.AreEqual(s, u.ToString());
        }

        [TestMethod]
        public void ParseBaseUnitPowered()
        {
            string s = "1m^2";
            var u = Unit.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseBaseUnitPrefixed()
        {
            string s = "1km";
            var u = Unit.Parse(s);
            Assert.AreEqual(s, u.ToString());
        }

        [TestMethod]
        public void ParseBaseUnitPrefixedPowered()
        {
            string s = "1km^3";
            var u = Unit.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseComplexUnit()
        {
            string s = "1m*s";
            var u = Unit.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseComplexUnitDivided()
        {
            string s = "1m/s";
            var u = Unit.Parse(s);
            Assert.AreEqual(s, u.ToString("d"));
        }

        [TestMethod]
        public void ParseComplexUnitPowered()
        {
            string s = "1m^2*s";
            var u = Unit.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseComplexUnitPoweredPrefixed()
        {
            string s = "1km^2*s^-1";
            var u = Unit.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseComplexUnitDividedPoweredPrefixed()
        {
            string s = "1m^3/s^2";
            var u = Unit.Parse(s);
            Assert.AreEqual(s, u.ToString("cdb"));
        }

        [TestMethod]
        public void ParseDerivedUnit()
        {
            string s = "1V";
            var u = Unit.Parse(s);
            Assert.AreEqual(s, u.ToString());
        }

        [TestMethod]
        public void ParseDerivedUnitPowered()
        {
            string s = "1V^2";
            var u = Unit.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseDerivedUnitPrefixed()
        {
            string s = "1mV";
            var u = Unit.Parse(s);
            Assert.AreEqual(s, u.ToString());
        }

        [TestMethod]
        public void ParseDerivedUnitPoweredPrefixed()
        {
            string s = "1mV^-3";
            var u = Unit.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseDerivedComplexUnitPoweredPrefixed()
        {
            string s = "1K*mV^-2";
            var u = Unit.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseDerivedLength2()
        {
            string s = "1Pa";
            var u = Unit.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseDerivedLength3PrefixedPowered()
        {
            string s = "1mkat^2";
            var u = Unit.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseMeasurementPrefixedPowered()
        {
            string s = 16.25 + "MT^2";
            var u = Unit.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ConvertSuperscript()
        {
            string s = "MT" + Stringifier.SS(2);
            var normal = UnitParser.ConvertSuperscript(s);
            Assert.AreEqual("MT^2", normal);
        }

        [TestMethod]
        public void ParseMixed()
        {
            string s = "12kmol^-1*MT^-2";
            var u = Unit.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }
 
    }
}
