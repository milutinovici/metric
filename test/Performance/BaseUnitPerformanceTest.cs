using BenchmarkDotNet.Attributes;
using Metric;

namespace Performance
{
    public class BaseUnitPerformanceTest
    {

        [Benchmark]
        public int NegativeFactorBase()
        {
            var u1 = new Unit(2, BaseUnit.s, -2);
            var u2 = new Unit(1, BaseUnit.s);
            return u1.HasFactor(u2);
        }

        [Benchmark]
        public bool EqualityBase()
        {
            var u1 = new Unit(1, BaseUnit.m);
            var u2 = new Unit(1, BaseUnit.m);
            return u1 == u2;
        }

        [Benchmark]
        public int CompareToBase()
        {
            var m = new Unit(999, BaseUnit.m);
            var km = new Unit(1, Prefix.k, BaseUnit.m);
            return m.CompareTo(km);
        }

        [Benchmark]
        public Unit AdditionBase()
        {
            var u1 = new Unit(1, BaseUnit.m);
            var u2 = new Unit(2, BaseUnit.m);
            return u1 + u2;
        }

        [Benchmark]
        public Unit SubstractionBase()
        {
            var u1 = new Unit(3, Prefix.k, BaseUnit.g);
            return u1 - u1;
        }
        
        [Benchmark]
        public Unit DivisionBase()
        {
            var u1 = new Unit(4, Prefix.m, BaseUnit.m, 2);
            var u2 = new Unit(2, Prefix.n, BaseUnit.m);
            return u1/u2;
        }
        
        [Benchmark]
        public Unit PowermentBase()
        {
            var u1 = new Unit(1, BaseUnit.m);
            return u1.Pow(-2);
        }

        [Benchmark]
        public Unit MultiplicationBase()
        {
            var u1 = new Unit(2, BaseUnit.m);
            return u1 * u1;
        }

        [Benchmark]
        public Unit ChangePrefixBase()
        {
            var m = new Unit(1, BaseUnit.m);
            return m.ChangePrefix(Prefix.k, BaseUnit.m);
        }

        [Benchmark]
        public Unit NumberAddition()
        {
            var u1 = new Unit(1, BaseUnit.K);
            return u1 + 100;
        }

        [Benchmark]
        public Unit NumberSubstraction()
        {
            var u1 = new Unit(1, BaseUnit.K);
            return u1 - 100;
        }

        [Benchmark]
        public Unit NumberMultiplication()
        {
            var u1 = new Unit(5, BaseUnit.K);
            return u1 * 4;
        }

        [Benchmark]
        public Unit NumberDivision()
        {
            var u1 = new Unit(1, BaseUnit.m);
            return  100 / u1;
        }
        
        [Benchmark]
        public Unit SquareRoot()
        {
            var u1 = new Unit(9, BaseUnit.m, 2);
            return u1.Pow(0.5);
        }
        
        [Benchmark]
        public string ToStringBase()
        {
            var u1 = new Unit(1, Prefix.k, BaseUnit.mol);
            return u1.ToString();
        }
    }
}