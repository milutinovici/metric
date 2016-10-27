namespace Metric.Extensions
{
    public static class DoubleExtensions
    {
        public static Unit A(this double number, sbyte power = 1) {
            return new Unit(number, BaseUnit.A, power);
        }
        public static Unit A(this double number, Prefix prefix, sbyte power = 1) {
            return new Unit(number, prefix, BaseUnit.A, power);
        }

        public static Unit cd(this double number, sbyte power = 1) {
            return new Unit(number, BaseUnit.cd, power);
        }
        public static Unit cd(this double number, Prefix prefix, sbyte power = 1) {
            return new Unit(number, prefix, BaseUnit.cd, power);
        }

        public static Unit g(this double number, sbyte power = 1) {
            return new Unit(number, BaseUnit.g, power);
        }
        public static Unit g(this double number, Prefix prefix, sbyte power = 1) {
            return new Unit(number, prefix, BaseUnit.g, power);
        }

        public static Unit K(this double number, sbyte power = 1) {
            return new Unit(number, BaseUnit.K, power);
        }
        public static Unit K(this double number, Prefix prefix, sbyte power = 1) {
            return new Unit(number, prefix, BaseUnit.K, power);
        }

        public static Unit m(this double number, sbyte power = 1) {
            return new Unit(number, BaseUnit.m, power);
        }
        public static Unit m(this double number, Prefix prefix, sbyte power = 1) {
            return new Unit(number, prefix, BaseUnit.m, power);
        }

        public static Unit mol(this double number, sbyte power = 1) {
            return new Unit(number, BaseUnit.mol, power);
        }
        public static Unit mol(this double number, Prefix prefix, sbyte power = 1) {
            return new Unit(number, prefix, BaseUnit.mol, power);
        }

        public static Unit s(this double number, sbyte power = 1) {
            return new Unit(number, BaseUnit.s, power);
        }
        public static Unit s(this double number, Prefix prefix, sbyte power = 1) {
            return new Unit(number, prefix, BaseUnit.s, power);
        }

        // derived units
        public static Unit ohm(this double number, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create("Ω");
            return number * Unit.Create("Ω").Pow(power);
        }
        public static Unit ohm(this double number, Prefix prefix, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create(prefix, "Ω");
            return number * Unit.Create(prefix, "Ω").Pow(power);
        }

        public static Unit V(this double number, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create("V");
            return number * Unit.Create("V").Pow(power);
        }
        public static Unit V(this double number, Prefix prefix, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create(prefix, "V");
            return number * Unit.Create(prefix, "V").Pow(power);
        }

        public static Unit H(this double number, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create("H");
            return number * Unit.Create("H").Pow(power);
        }
        public static Unit H(this double number, Prefix prefix, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create(prefix, "H");
            return number * Unit.Create(prefix, "H").Pow(power);
        }

        public static Unit Wb(this double number, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create("Wb");
            return number * Unit.Create("Wb").Pow(power);
        }
        public static Unit Wb(this double number, Prefix prefix, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create(prefix, "Wb");
            return number * Unit.Create(prefix, "Wb").Pow(power);
        }

        public static Unit F(this double number, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create("F");
            return number * Unit.Create("F").Pow(power);
        }
        public static Unit F(this double number, Prefix prefix, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create(prefix, "F");
            return number * Unit.Create(prefix, "F").Pow(power);
        }

        public static Unit S(this double number, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create("S");
            return number * Unit.Create("S").Pow(power);
        }
        public static Unit S(this double number, Prefix prefix, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create(prefix, "S");
            return number * Unit.Create(prefix, "S").Pow(power);
        }

        public static Unit W(this double number, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create("W");
            return number * Unit.Create("W").Pow(power);
        }
        public static Unit W(this double number, Prefix prefix, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create(prefix, "W");
            return number * Unit.Create(prefix, "W").Pow(power);
        }

        public static Unit J(this double number, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create("J");
            return number * Unit.Create("J").Pow(power);
        }
        public static Unit J(this double number, Prefix prefix, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create(prefix, "J");
            return number * Unit.Create(prefix, "J").Pow(power);
        }

        public static Unit N(this double number, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create("N");
            return number * Unit.Create("N").Pow(power);
        }
        public static Unit N(this double number, Prefix prefix, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create(prefix, "N");
            return number * Unit.Create(prefix, "N").Pow(power);
        }

        public static Unit Pa(this double number, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create("Pa");
            return number * Unit.Create("Pa").Pow(power);
        }
        public static Unit Pa(this double number, Prefix prefix, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create(prefix, "Pa");
            return number * Unit.Create(prefix, "Pa").Pow(power);
        }

        public static Unit T(this double number, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create("T");
            return number * Unit.Create("T").Pow(power);
        }
        public static Unit T(this double number, Prefix prefix, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create(prefix, "T");
            return number * Unit.Create(prefix, "T").Pow(power);
        }

        public static Unit C(this double number, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create("C");
            return number * Unit.Create("C").Pow(power);
        }
        public static Unit C(this double number, Prefix prefix, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create(prefix, "C");
            return number * Unit.Create(prefix, "C").Pow(power);
        }

        public static Unit Gy(this double number, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create("Gy");
            return number * Unit.Create("Gy").Pow(power);
        }
        public static Unit Gy(this double number, Prefix prefix, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create(prefix, "Gy");
            return number * Unit.Create(prefix, "Gy").Pow(power);
        }

        public static Unit lx(this double number, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create("lx");
            return number * Unit.Create("lx").Pow(power);
        }
        public static Unit lx(this double number, Prefix prefix, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create(prefix, "lx");
            return number * Unit.Create(prefix, "lx").Pow(power);
        }

        public static Unit kat(this double number, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create("kat");
            return number * Unit.Create("kat").Pow(power);
        }
        public static Unit kat(this double number, Prefix prefix, sbyte power = 1) {
            if(power == 1)
                return number * Unit.Create(prefix, "kat");
            return number * Unit.Create(prefix, "kat").Pow(power);
        }

    }
}