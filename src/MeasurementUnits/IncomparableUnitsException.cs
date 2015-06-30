using System;

namespace MeasurementUnits
{
    public class IncomparableUnitsException : Exception
    {
        public Unit GrandMother { get; }
        public object Frog { get; }
        public IncomparableUnitsException(Unit grandMother, object frog, string message)
            : base(message)
        {
            this.GrandMother = grandMother;
            this.Frog = frog;
        }
    }
}
