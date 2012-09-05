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
    public class ComplexUnitTest
    {
        public Unit N { get; set; }
        public Unit S { get; set; }


        public ComplexUnitTest()
        {
            N = new Unit(new Unit(BaseUnit.m), new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.s, -2));
            S = new Unit(BaseUnit.s);
        }

        [TestMethod]
        public void IsAddableComplexWithDifferentPrefixes()
        {
            var kn = new Unit(new Unit(Prefix.k, BaseUnit.g), new Unit(Prefix.k, BaseUnit.m), new Unit(BaseUnit.s, -2));
            Assert.IsTrue(N.IsAddable(kn));
        }

        [TestMethod]
        public void MultiplicationOfComplexAndBaseUnit()
        {
            Assert.AreEqual("1kg" + Stringifier.dot + "m" + Stringifier.dot + "s" + Stringifier.SS(-1), (N * S).ToString());
        }
        [TestMethod]
        public void DivisionOfComplexAndBaseUnit()
        {
            Assert.AreEqual("1N" + Stringifier.dot + "s" + Stringifier.SS(-1), (N / S).ToString());
        }

        [TestMethod]
        public void DivisionOf2ComplexUnits()
        {
            var u = new Unit(new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m), new Unit(BaseUnit.s, -1));
            var u1 = N / u;
            Assert.AreEqual("1s" + Stringifier.SS(-1), u1.ToString());
        }

        [TestMethod]
        public void HasFactorBaseUnit()
        {
            int power = 0;
            var u = N.HasFactor(S, ref power);
            Assert.AreEqual(-2, power);
        }

        [TestMethod]
        public void HasFactorComplexUnit()
        {
            int power = 0;
            var u = N.Pow(2).HasFactor(N, ref power);
            Assert.AreEqual(2, power);
        }

        [TestMethod]
        public void HasFactorNegativeComplexUnit()
        {
            int power = 0;
            var u = N.HasFactor(N.Pow(-1), ref power);
            Assert.AreEqual(-1, power);
        }

        [TestMethod]
        public void RecognizeDerivedUnit()
        {
            Assert.AreEqual("1N", N.ToString());
        }

        [TestMethod]
        public void RecognizeDerivedUnitWithPrefix()
        {
            var kn = new Unit(1, new Unit(Prefix.k, BaseUnit.g), new Unit(Prefix.k, BaseUnit.m), new Unit(BaseUnit.s, -2));
            Assert.AreEqual("1kN", kn.ToString());
        }

        [TestMethod]
        public void MultiplyDerivedUnits()
        {
            Unit c = N * N;
            Assert.AreEqual("1N" + Stringifier.SS(2), c.ToString());
        }

        [TestMethod]
        public void MultiplyDerivedUnitsWithPrefix()
        {
            var kn = new Unit(new Unit(Prefix.k, BaseUnit.g), new Unit(Prefix.k, BaseUnit.m), new Unit(BaseUnit.s, -2));
            var n = new Unit(new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m), new Unit(BaseUnit.s, -2));
            var c = n * kn;
            Assert.AreEqual("10daN" + Stringifier.SS(2), c.ToString());
        }

        [TestMethod]
        public void ComplexDerivedUnit()
        {
            Unit c = (N / S);
            Assert.AreEqual("1N" + Stringifier.dot + "s" + Stringifier.SS(-1), c.ToString());
        }

        [TestMethod]
        public void PrefixTransferDown()
        {
            Unit n = new Unit(new Unit(BaseUnit.m), new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.s, -2));
            n = n.ChangePrefix(Prefix.m);
            var u = n.Units.First();
            Assert.AreEqual(Prefix.m, n.Prefix);
        }

        [TestMethod]
        public void ParseDerivedUnitPoweredPrefixed()
        {
            string s = "1mV^-3";
            var u = Unit.Parse(s).Pow(-1);
            Assert.AreEqual("1mV^3", u.ToString("c"));
        }

        [TestMethod]
        public void RecognizeDerivedUnitWithPrefix2()
        {
            var u = new Unit(2, new Unit(Prefix.h, BaseUnit.g), new Unit(Prefix.M, BaseUnit.m), new Unit(BaseUnit.s, -2));
            Assert.AreEqual("200kN", u.ToString());
        }
        [TestMethod]
        public void kWEquals1000W()
        {
            var w = new Unit(1000, new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -3));
            var kw = new Unit(1, new Unit(Prefix.M, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -3));
            Assert.IsTrue(w.Equals(kw));
        }

        //[TestMethod]
        //public void kJTo1000J()
        //{
        //    var kJ = Unit.Parse("1kJ");
        //    kJ.Prefix = 0;
        //    Assert.AreEqual("1000J", kJ.ToString());
        //}
    }
}

