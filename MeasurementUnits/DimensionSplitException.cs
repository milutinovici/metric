using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
