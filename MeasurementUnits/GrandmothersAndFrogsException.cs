using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeasurementUnits
{
    public class GrandmothersAndFrogsException : Exception
    {
        public Unit GrandMother { get; private set; }
        public Unit Frog { get; private set; }
        public GrandmothersAndFrogsException(Unit grandMother, Unit frog, string message)
            : base(message)
        {

        }
    }
}
