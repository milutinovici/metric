using System;

namespace Metric
{
    public class IncomparableUnitsException : Exception
    {
        public Unit Unit1 { get; }
        public object Unit2 { get; }
        public IncomparableUnitsException(Unit u1, object u2)
            : base($"Units {u1} and {u2} are incomparable.")
        {
            this.Unit1 = u1;
            this.Unit2 = u2;
        }
    }
}
