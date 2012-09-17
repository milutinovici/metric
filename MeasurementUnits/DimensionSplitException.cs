using System;

namespace MeasurementUnits
{
    public class DimensionSplitException : Exception
    {
        public Unit DimensionSplit { get; private set; }
        public DimensionSplitException(Unit dimensionSplit, string message)
            : base(message)
        {
            DimensionSplit = dimensionSplit;
        }
    }
}
