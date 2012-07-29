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
            Assert.AreEqual("m", M.ToString());
        }

        [TestMethod]
        public void ToStringWithPrefix()
        {
            Assert.AreEqual("kg", Kg.ToString());
        }

        [TestMethod]
        public void ToStringWithPrefixAndPower()
        {
            Assert.AreEqual("kg" + Str.SS(2), Kg.Pow(2).ToString());
        }

        [TestMethod]
        public void Addition()
        {
            Assert.AreEqual("m", (M + M).ToString());
        }

        [TestMethod]
        public void SubstractionWithPrefix()
        {
            Assert.AreEqual("kg", (Kg - Kg).ToString());
        }

        [TestMethod]
        public void Powerment()
        {
            Assert.AreEqual("m" + Str.SS(2), M.Pow(2).ToString());
        }

        [TestMethod]
        public void SelfMultiplication()
        {
            Assert.AreEqual("m" + Str.SS(2), (M * M).ToString());
        }

        [TestMethod]
        public void SelfDivision()
        {
            Assert.AreEqual("", (M / M).ToString());
        }

        [TestMethod]
        public void GMultiplication()
        {
            Assert.AreEqual("m" + Str.dot + "s", (M * S).ToString());
        }

        [TestMethod]
        public void FindClosestPrefix()
        {
            var prfx = Unit.FindClosestPrefix(-59);
            Assert.AreEqual(Prefix.y, prfx);
        }

    }
}
