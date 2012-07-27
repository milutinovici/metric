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

        [TestMethod]
        public void TestToString()
        {
            Assert.AreEqual(M.ToString(), "m");
        }

        [TestMethod]
        public void ToStringWithPrefix()
        {
            Assert.AreEqual(Kg.ToString(), "kg");
        }

        [TestMethod]
        public void ToStringWithPrefixAndPower()
        {
            Assert.AreEqual(Kg.Pow(2).ToString(), "kg" + Str.SS(2));
        }

        [TestMethod]
        public void Addition()
        {
            Assert.AreEqual((M + M).ToString(), "m");
        }

        [TestMethod]
        public void SubstractionWithPrefix()
        {
            Assert.AreEqual((Kg - Kg).ToString(), "kg");
        }

        [TestMethod]
        public void Powerment()
        {
            Assert.AreEqual(M.Pow(2).ToString(), "m" + Str.SS(2));
        }

        [TestMethod]
        public void SelfMultiplication()
        {
            Assert.AreEqual((M * M).ToString(), "m" + Str.SS(2));
        }

        [TestMethod]
        public void SelfDivision()
        {
            Assert.AreEqual((M / M).ToString(), "");
        }

        [TestMethod]
        public void GMultiplication()
        {
            Assert.AreEqual((M * S).ToString(), "m" + Str.dot + "s");
        }

        [TestMethod]
        public void FindClosestPrefix()
        {
            var prfx = Unit.FindClosestPrefix(-59);
            Assert.AreEqual(prfx, Prefix.y);
        }

    }
}
