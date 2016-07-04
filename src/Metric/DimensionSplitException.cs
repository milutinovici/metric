using System;

namespace Metric
{
    public class DimensionSplitException : Exception
    {
        public Unit DimensionSplit { get; }
        public double Power { get; }
        public DimensionSplitException(Unit dimensionSplit, double power)
            : base($"Powering unit {dimensionSplit} with {power}, would result with non integer power.")
        {
            Power = power;
            DimensionSplit = dimensionSplit;
        }
    }
}
