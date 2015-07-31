using System;

namespace Metric
{
    public class DimensionSplitException : Exception
    {
        public Unit DimensionSplit { get; }
        public DimensionSplitException(Unit dimensionSplit, string message)
            : base(message)
        {
            DimensionSplit = dimensionSplit;
        }
    }
}
