using BenchmarkDotNet.Attributes;
using Metric;

namespace Performance
{
    public class BaseUnitPerformanceTest
    {

        [Benchmark]
        public string Meter()
        {
            var u = Unit.Create("W");
            return u.ToString();
        }

    }
}