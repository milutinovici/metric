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
            string s = "m";
            var u = UnitParser.Parse(s);
            Assert.AreEqual(s, u.ToString());
        }

        [TestMethod]
        public void ParseBaseUnitPowered()
        {
            string s = "m^2";
            var u = UnitParser.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseBaseUnitPrefixed()
        {
            string s = "km";
            var u = UnitParser.Parse(s);
            Assert.AreEqual(s, u.ToString());
        }

        [TestMethod]
        public void ParseBaseUnitPrefixedPowered()
        {
            string s = "km^3";
            var u = UnitParser.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseComplexUnit()
        {
            string s = "m*s";
            var u = UnitParser.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseComplexUnitDivided()
        {
            string s = "m/s";
            var u = UnitParser.Parse(s);
            Assert.AreEqual(s, u.ToString("d"));
        }

        [TestMethod]
        public void ParseComplexUnitPowered()
        {
            string s = "m^2*s";
            var u = UnitParser.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseComplexUnitPoweredPrefixed()
        {
            string s = "km^2*s^-1";
            var u = UnitParser.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseComplexUnitDividedPoweredPrefixed()
        {
            string s = "m^3/s^2";
            var u = UnitParser.Parse(s);
            Assert.AreEqual(s, u.ToString("cd"));
        }

        [TestMethod]
        public void ParseDerivedUnit()
        {
            string s = "V";
            var u = UnitParser.Parse(s);
            Assert.AreEqual(s, u.ToString());
        }

        [TestMethod]
        public void ParseDerivedUnitPowered()
        {
            string s = "V^2";
            var u = UnitParser.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseDerivedUnitPrefixed()
        {
            string s = "mV";
            var u = UnitParser.Parse(s);
            Assert.AreEqual(s, u.ToString());
        }

        [TestMethod]
        public void ParseDerivedUnitPoweredPrefixed()
        {
            string s = "mV^-3";
            var u = UnitParser.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseDerivedComplexUnitPoweredPrefixed()
        {
            string s = "mol^2*mV^-3";
            var u = UnitParser.Parse(s) as ComplexUnit;
            u.FindDerivedUnits();
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseDerivedLength2()
        {
            string s = "Pa";
            var u = UnitParser.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseDerivedLength3PrefixedPowered()
        {
            string s = "mkat^2";
            var u = UnitParser.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        [TestMethod]
        public void ParseMeasurementPrefixedPowered()
        {
            string s = 16.25 + "MT^2";
            var u = MeasurementUnit.Parse(s);
            Assert.AreEqual(s, u.ToString("c"));
        }

        //[TestMethod]
        //public void ConvertSuperscript()
        //{
        //    string s = "MT" + Str.SS(2);
        //    var normal = UnitParser.ConvertSuperscript(s);
        //    Assert.AreEqual("MT^2", normal);
        //}

        //[TestMethod]
        //public void ParseMixed()
        //{
        //    string s = "12MT" + Str.SS(-2) + Str.dot + "kmol^-1";
        //    var u = MeasurementUnit.Parse(s);
        //    Assert.AreEqual(s, u.ToString());
        //}
 
    }
}
