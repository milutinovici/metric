using System;

namespace MeasurementUnits
{
    public class IncomparableUnitsException : Exception
    {
        public Unit Unit1 { get; }
        public Unit Unit2 { get; }
        public IncomparableUnitsException(Unit grandMother, Unit frog, string message)
            : base(message)
        {
            this.Unit1 = grandMother;
            this.Unit2 = frog;
        }
    }
}
