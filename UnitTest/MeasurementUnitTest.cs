using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MeasurementUnits;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class MeasurementUnitTest
    {
        public MeasurementUnit M { get; set; }
        public MeasurementUnit Km { get; set; }
        public MeasurementUnit Kg { get; set; }
        public MeasurementUnit Sm2 { get; set; }
        public MeasurementUnit N { get; set; }
        public MeasurementUnit Nm1 { get; set; }
        public MeasurementUnit K3 { get; set; }
        public MeasurementUnit AKMol { get; set; }

        public MeasurementUnitTest()
        {
            M = new MeasurementUnit(5, new Unit(BaseUnit.m));
            Km = new MeasurementUnit(12, new Unit(Prefix.k, BaseUnit.m));
            Kg = new MeasurementUnit(3, new Unit(Prefix.k, BaseUnit.g));
            Sm2 = new MeasurementUnit(2, new Unit(BaseUnit.s, -2));
            N = new MeasurementUnit(6, new ComplexUnit(new Unit(BaseUnit.m), new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.s, -2)));
            Nm1 = new MeasurementUnit(4, new ComplexUnit(new Unit(BaseUnit.m, -1), new Unit(Prefix.k, BaseUnit.g, -1), new Unit(BaseUnit.s, 2)));
            K3 = new MeasurementUnit(18, new Unit(BaseUnit.K, 3));
            AKMol = new MeasurementUnit(4, new ComplexUnit(new Unit(BaseUnit.K), new Unit(BaseUnit.A), new Unit(BaseUnit.mol, -1)));
        }
         
        [TestMethod]
        public void WithPositivePower()
        {
            Assert.AreEqual("18 K" + Str.SS(3), K3.ToString());
        }

        [TestMethod]
        public void WithNegativePower()
        {
            Assert.AreEqual("2 s" + Str.SS(-2), Sm2.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(GrandmothersAndFrogsException))]
        public void AdditionOfGrandmothersAndFrogs()
        {
            var unit = Kg + N;
        }

        [TestMethod]
        public void AdditionWithDifferentPrefixes()
        {
            Assert.AreEqual("120.05 hm", (M + Km).ToString());
        }

        [TestMethod]
        public void SubtractionWithDifferentPrefixes()
        {
            Assert.AreEqual("-119.95 hm", (M - Km).ToString());
        }

        [TestMethod]
        public void Multiplication()
        {
            Assert.AreEqual("20 m" + Str.dot + "A" + Str.dot + "K" + Str.dot + "mol" + Str.SS(-1), (AKMol * M).ToString());
        }

        [TestMethod]
        public void MultiplicationWithDifferentPrefixes()
        {
            Assert.AreEqual("300 dam" + Str.SS(3), (M * Km * M).ToString());
        }

        [TestMethod]
        public void DoubleMultiplication()
        {
            MeasurementUnit u = AKMol * K3 * M;
            Assert.AreEqual("360 m" + Str.dot + "A" + Str.dot + "K" + Str.SS(4) + Str.dot + "mol" + Str.SS(-1), u.ToString());
        }

        [TestMethod]
        public void MultiplicationThenDivision()
        {
            MeasurementUnit u = AKMol * K3 / M;
            Assert.AreEqual("14.4 A" + Str.dot + "K" + Str.SS(4) + Str.dot + "m" + Str.SS(-1) + Str.dot + "mol" + Str.SS(-1), u.ToString());
        }

        [TestMethod]
        public void MultiplicationThenSubtractionWithPrefixes()
        {
            var u1 = M * Km - M * M;
            Assert.AreEqual("599.75 dam" + Str.SS(2), u1.ToString());
        }

        [TestMethod]
        public void RecognizeDerivedUnit()
        {
            Assert.AreEqual("6 N", N.ToString());
        }

        [TestMethod]
        public void RecognizeDerivedUnit2()
        {
            Assert.AreEqual("36 N" + Str.SS(2), (N * N).ToString());
        }

        [TestMethod]
        public void RecognizeDerivedUnitWithNegativeExponent()
        {
            Assert.AreEqual("4 N" + Str.SS(-1), Nm1.ToString());
        }

        [TestMethod]
        public void RecognizeDerivedUnit2WithPrefix()
        {
            var kn = new MeasurementUnit(4, new ComplexUnit(new Unit(Prefix.k, BaseUnit.g), new Unit(Prefix.k, BaseUnit.m), new Unit(BaseUnit.s, -2)));
            Assert.AreEqual("2.4 hN" + Str.SS(2), (N * kn).ToString());
        }

        [TestMethod]
        public void RecognizeDerivedUnitWithPrefix()
        {
            var u = new MeasurementUnit(2, new ComplexUnit(new Unit(Prefix.k, BaseUnit.g), new Unit(Prefix.k, BaseUnit.m), new Unit(BaseUnit.s,-2 )));
            Assert.AreEqual("2 kN", u.ToString());
        }

        [TestMethod]
        public void RecognizeDerivedUnitWithPrefix2()
        {
            var u = new MeasurementUnit(2, new ComplexUnit(new Unit(Prefix.h, BaseUnit.g), new Unit(Prefix.M, BaseUnit.m), new Unit(BaseUnit.s, -2)));
            Assert.AreEqual("200 kN", u.ToString());
        }

        [TestMethod]
        public void kmEquals1000m()
        {
            var m = new MeasurementUnit(1000, new Unit(BaseUnit.m));
            var km = new MeasurementUnit(1, new Unit(Prefix.k, BaseUnit.m));
            Assert.IsTrue(m.Equals(km));
        }

        [TestMethod]
        public void kWEquals1000W()
        {
            var w = new MeasurementUnit(1000, new ComplexUnit(new Unit(Prefix.k, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -3)));
            var kw = new MeasurementUnit(1, new ComplexUnit(new Unit(Prefix.M, BaseUnit.g), new Unit(BaseUnit.m, 2), new Unit(BaseUnit.s, -3)));
            Assert.IsTrue(w.Equals(kw));
        } 
    }
}
