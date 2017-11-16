using System.Text;

namespace Metric
{
    public struct SingleUnit
    {
        static readonly string[] SuperscriptDigits = { "\u2070", "\u00b9", "\u00b2", "\u00b3", "\u2074", "\u2075", "\u2076", "\u2077", "\u2078", "\u2079" };
        public static readonly string Dot = "\u00B7";
        public static readonly string Minus = "\u207B";

        public Prefix Prefix { get; }
        public string Symbol { get; }
        public sbyte Power { get; }

        public SingleUnit(Prefix prefix, string symbol, sbyte power)
        {
            this.Prefix = prefix;
            this.Symbol = symbol;
            this.Power = power;
        }

        public SingleUnit Reciprocal()
        {
            return new SingleUnit(Prefix, Symbol, (sbyte)-Power);
        }

        public string ToString(bool fancy)
        {
            var s = new StringBuilder();
            if (Power == 0)
                return "";
            if (Prefix != 0)
                s.Append(Prefix);
            s.Append(Symbol);
            if (Power != 1)
            {
                if (fancy)
                    s.Append(SS(Power));
                else
                    s.Append("^").Append(Power);
            }
            return s.ToString();
        }
        public override string ToString()
        {
            return ToString(true);
        }

        public static string SS(int power)
        {
            var sb = new StringBuilder();
            if (power < 0)
            {
                sb.Append(Minus);
                power *= -1;
            }

            foreach (var ch in power.ToString())
            {
                int num = (int)char.GetNumericValue(ch);
                sb.Append(SuperscriptDigits[num]);
            }
            return sb.ToString();
        }
    }
}
