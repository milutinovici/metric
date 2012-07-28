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
            N = new ComplexUnit(new Unit(BaseUnit.m), new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.s, -2));
            S = new Unit(BaseUnit.s);
        }

        [TestMethod]
        public void IsAddableComplexWithDifferentPrefixes()
        {
            var kn = new ComplexUnit(new Unit(Prefix.k, BaseUnit.g), new Unit(Prefix.k, BaseUnit.m), new Unit(BaseUnit.s, -2));
            Assert.IsTrue(N.IsAddable(kn));
        }

        [TestMethod]
        public void MultiplicationOfComplexAndBaseUnit()
        {
            Assert.AreEqual((N * S).ToString(), "m" + Str.dot + "kg" + Str.dot + "s" + Str.SS(-1));
        }
        [TestMethod]
        public void DivisionOfComplexAndBaseUnit()
        {
            Assert.AreEqual((N / S).ToString(), "m" + Str.dot + "kg" + Str.dot + "s" + Str.SS(-3));
        }

        [TestMethod]
        public void DivisionOf2ComplexUnits()
        {
            var u = new ComplexUnit(new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m), new Unit(BaseUnit.s, -1));
            var u1 = N / u;
            Assert.AreEqual(u1.ToString(), "s" + Str.SS(-1));
        }

        [TestMethod]
        public void HasFactorBaseUnit()
        {
            int power = 0;
            var u = N.HasFactor(S, ref power);
            Assert.AreEqual(power, -2);
        }

        [TestMethod]
        public void HasFactorComplexUnit()
        {
            int power = 0;
            var u = N.Pow(2).HasFactor(N, ref power);
            Assert.AreEqual(power, 2);
        }

        [TestMethod]
        public void HasFactorNegativeComplexUnit()
        {
            int power = 0;
            var u = N.HasFactor(N.Pow(-1), ref power);
            Assert.AreEqual(power, -1);
        }

        [TestMethod]
        public void RecognizeDerivedUnit()
        {
            ComplexUnit c = (ComplexUnit)N;
            c.FindDerivedUnits();
            Assert.AreEqual(c.ToString(), "N");
        }

        [TestMethod]
        public void RecognizeDerivedUnitWithPrefix()
        {
            var kn = new ComplexUnit(new Unit(Prefix.k, BaseUnit.g), new Unit(Prefix.k, BaseUnit.m), new Unit(BaseUnit.s, -2));
            kn.FindDerivedUnits();
            Assert.AreEqual(kn.ToString(), "kN");
        }

        [TestMethod]
        public void MultiplyDerivedUnits()
        {
            ComplexUnit c = (ComplexUnit)(N * N);
            c.FindDerivedUnits();
            Assert.AreEqual(c.ToString(), "N" + Str.SS(2));
        }

        [TestMethod]
        public void MultiplyDerivedUnitsWithPrefix()
        {
            var kn = new ComplexUnit(new Unit(Prefix.k, BaseUnit.g), new Unit(Prefix.k, BaseUnit.m), new Unit(BaseUnit.s, -2));
            var n = new ComplexUnit(new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m), new Unit(BaseUnit.s, -2));
            ComplexUnit c = (ComplexUnit)(n * kn);
            c.FindDerivedUnits();
            Assert.AreEqual(c.ToString(), "hN" + Str.SS(2));
        }

        [TestMethod]
        public void ComplexDerivedUnit()
        {
            ComplexUnit c = (ComplexUnit)(N / S);
            c.FindDerivedUnits();
            Assert.AreEqual(c.ToString(), "N" + Str.dot + "s" + Str.SS(-1));
        }

        [TestMethod]
        public void PrefixTransferDown()
        {
            ComplexUnit n = new ComplexUnit(new Unit(BaseUnit.m), new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.s, -2));
            n.Prefix = Prefix.m;
            var u = n.Units.First();
            Assert.AreEqual(n.Prefix, Prefix.m);
        }

        [TestMethod]
        public void PrefixTransferUp()
        {
            ComplexUnit n = new ComplexUnit(new Unit(BaseUnit.m), new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.s, -2));
            n.Units.First().Prefix = Prefix.M;
            Assert.AreEqual(n.Prefix, Prefix.M);
        }

        [TestMethod]
        public void PrefixTransferUpM2()
        {
            ComplexUnit n = new ComplexUnit(new Unit(BaseUnit.m), new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.s, -2));
            n.Units.Last().Prefix = Prefix.k;
            Assert.AreEqual(n.Prefix, Prefix.μ);
        }

        [TestMethod]
        public void PrefixTransferUpWithRounding()
        {
            ComplexUnit n = new ComplexUnit(new Unit(BaseUnit.m), new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.s, -2));
            n.Units.Last().Prefix = Prefix.c;
            Assert.AreEqual(n.Prefix, Prefix.k);
        }

    }
}

