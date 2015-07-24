using System;

namespace MeasurementUnits
{
    public class IncomparableUnitsException : Exception
    {
        public Unit Unit1 { get; }
        public object Unit2 { get; }
        public IncomparableUnitsException(Unit u1, object u2, string message)
            : base(message)
        {
            this.Unit1 = u1;
            this.Unit2 = u2;
        }
    }
}
