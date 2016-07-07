using System;
using BenchmarkDotNet.Running;

namespace Performance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<BaseUnitPerformanceTest>();
        }
    }
}
