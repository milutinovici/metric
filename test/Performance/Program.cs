using BenchmarkDotNet.Running;

namespace Performance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var s1 = BenchmarkRunner.Run<BaseUnitPerformanceTest>();
            var s2 = BenchmarkRunner.Run<ComplexUnitPerformanceTest>();
        }
    }
}
