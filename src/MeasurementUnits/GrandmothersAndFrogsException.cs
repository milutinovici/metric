using System;

namespace MeasurementUnits
{
    public class GrandmothersAndFrogsException : Exception
    {
        public Unit GrandMother { get; }
        public object Frog { get; }
        public GrandmothersAndFrogsException(Unit grandMother, object frog, string message)
            : base(message)
        {
            this.GrandMother = grandMother;
            this.Frog = frog;
        }
    }
}
