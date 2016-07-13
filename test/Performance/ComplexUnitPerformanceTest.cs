using BenchmarkDotNet.Attributes;
using Metric;

namespace Performance
{
    public class ComplexUnitPerformanceTest
    {
        public Unit N { get; set; }
        public Unit S { get; set; }


        public ComplexUnitPerformanceTest()
        {
            N = new Unit(1, BaseUnit.m) * new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.s, -2);
            S = new Unit(1, BaseUnit.s);
        }

        [Benchmark]
        public bool IsComparableComplex()
        {
            var kn = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, Prefix.k, BaseUnit.m) * new Unit(1, BaseUnit.s, -2);
            return N.IsComparable(kn);
        }

        [Benchmark]
        public Unit MultiplicationComplex()
        {
            var u1 = new Unit(1, Prefix.k, BaseUnit.m) * new Unit(1, BaseUnit.s, -1);
            var u2 = new Unit(1, Prefix.m, BaseUnit.s, 2) * new Unit(1, BaseUnit.m, -1);
            return u1 * u2;
        }
        
        [Benchmark]
        public int HasFactorComplex()
        {
            return N.Pow(2).HasFactor(N);
        }

        [Benchmark]
        public int HasFactorNegativeComplex()
        {
            return N.HasFactor(N.Pow(-1));
        }

        [Benchmark]
        public Unit MultiplyDerived()
        {
            var kn = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, Prefix.k, BaseUnit.m) * new Unit(1, BaseUnit.s, -2);
            var n = new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m) * new Unit(1, BaseUnit.s, -2);
            return n * kn;
        }
        
        [Benchmark]
        public bool EqualsDerived()
        {
            var w = new Unit(1000, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s, -3);
            var kw = new Unit(1, Prefix.M, BaseUnit.g) * new Unit(1, BaseUnit.m, 2) * new Unit(1, BaseUnit.s, -3);
            return w == kw;
        }

        [Benchmark]
        public string RecognizeDerivedUnit()
        {
            var mn = new Unit(12, BaseUnit.m) * new Unit(1, Prefix.k, BaseUnit.g) * new Unit(1, BaseUnit.s, -2);
            return mn.ToString(); 
        }

        [Benchmark]
        public string RecognizeDerivedUnitWithPrefix()
        {
            var mn = new Unit(5, BaseUnit.g) * new Unit(1, BaseUnit.m) * new Unit(1, BaseUnit.s, -2);
            return mn.ToString();
        }
        
        [Benchmark]
        public string WithDivisor()
        {
            var u = new Unit(1, Prefix.m, BaseUnit.m) * new Unit(1, BaseUnit.s, -2);
            return u.ToString("dc");
        }

        [Benchmark]
        public string CreateDerivedUnitPrefixed()
        {
            var u = Unit.Create(Prefix.m, "F");
            return u.ToString();
        }

        [Benchmark]
        public string CreateDerivedUnitPrefixed2()
        {
            var u = Unit.Create(Prefix.m, "Gy");
            return u.ToString();
        }
    }
}