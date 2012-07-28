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
            Assert.AreEqual(K3.ToString(), "18 K" + Str.SS(3));
        }

        [TestMethod]
        public void WithNegativePower()
        {
            Assert.AreEqual(Sm2.ToString(), "2 s" + Str.SS(-2));
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
            Assert.AreEqual((M + Km).ToString(), "120.05 hm");
        }

        [TestMethod]
        public void SubtractionWithDifferentPrefixes()
        {
            Assert.AreEqual((M - Km).ToString(), "-119.95 hm");
        }

        [TestMethod]
        public void Multiplication()
        {
            Assert.AreEqual((AKMol * M).ToString(), "20 m" + Str.dot + "A" + Str.dot + "K" + Str.dot + "mol" + Str.SS(-1));
        }

        [TestMethod]
        public void MultiplicationWithDifferentPrefixes()
        {
            Assert.AreEqual((M * Km * M).ToString(), "300 dam" + Str.SS(3));
        }

        [TestMethod]
        public void DoubleMultiplication()
        {
            MeasurementUnit u = AKMol * K3 * M;
            Assert.AreEqual(u.ToString(), "360 m" + Str.dot + "A" + Str.dot + "K" + Str.SS(4) + Str.dot + "mol" + Str.SS(-1));
        }

        [TestMethod]
        public void MultiplicationThenDivision()
        {
            MeasurementUnit u = AKMol * K3 / M;
            Assert.AreEqual(u.ToString(), "14.4 A" + Str.dot + "K" + Str.SS(4) + Str.dot + "m" + Str.SS(-1) + Str.dot + "mol" + Str.SS(-1));
        }

        [TestMethod]
        public void MultiplicationThenSubtractionWithPrefixes()
        {
            var u1 = M * Km - M * M;
            Assert.AreEqual(u1.ToString(), "599.75 dam" + Str.SS(2));
        }

        [TestMethod]
        public void RecognizeDerivedUnit()
        {
            Assert.AreEqual(N.ToString(), "6 N");
        }

        [TestMethod]
        public void RecognizeDerivedUnit2()
        {
            Assert.AreEqual((N * N).ToString(), "36 N" + Str.SS(2));
        }

        [TestMethod]
        public void RecognizeDerivedUnitWithNegativeExponent()
        {
            Assert.AreEqual(Nm1.ToString(), "4 N" + Str.SS(-1));
        }

        [TestMethod]
        public void RecognizeDerivedUnit2WithPrefix()
        {
            var kn = new MeasurementUnit(4, new ComplexUnit(new Unit(Prefix.k, BaseUnit.g), new Unit(Prefix.k, BaseUnit.m), new Unit(BaseUnit.s, -2)));
            Assert.AreEqual((N * kn).ToString(), "2.4 hN" + Str.SS(2));
        }

        [TestMethod]
        public void RecognizeDerivedUnitWithPrefix()
        {
            var u = new MeasurementUnit(2, new ComplexUnit(new Unit(Prefix.k, BaseUnit.g), new Unit(Prefix.k, BaseUnit.m), new Unit(BaseUnit.s,-2 )));
            Assert.AreEqual(u.ToString(), "2 kN");
        }

        [TestMethod]
        public void RecognizeDerivedUnitWithPrefix2()
        {
            var u = new MeasurementUnit(2, new ComplexUnit(new Unit(Prefix.h, BaseUnit.g), new Unit(Prefix.M, BaseUnit.m), new Unit(BaseUnit.s, -2)));
            Assert.AreEqual(u.ToString(), "200 kN");
        }

        [TestMethod]
        public void kmEquals1000m()
        {
            var m = new MeasurementUnit(1000, new Unit(BaseUnit.m));
            var km = new MeasurementUnit(1, new Unit(Prefix.k, BaseUnit.m));
            Assert.AreEqual(m, km);
        }

    }
}
