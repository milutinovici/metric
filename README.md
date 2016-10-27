# Metric.NET #

**Metric.NET** is a library written in C#, designed to help developers who are dealing with measurement units. It allows you to do calculations and the library figures out resulting units.

[Nuget](https://www.nuget.org/packages/Metric)

## Usage ##
There are 2 enums and 1 immutable struct that you need to know.

- BaseUnit - enum which represents all base SI units (meters,seconds,grams..) 
- Prefix - enum which represents multiples (kilo, mega, mili, micro...)
- Unit - struct that brings it all together

Want to instantiate  8km^3? You can do it like so:
> `var volume = new Unit(8, Prefix.k, BaseUnit.m, 3);`

Or you can parse it from a string:
> `var area = Unit.Parse("4km^2");`

You can also use extension methods for int and double, located in Metric.Extensions

> `var width = 6.m();`

Now that you have these units you can multiply or divide them

> `var height = volume / area;`

result will be equal to 2km

Power them:

> `var length = area.Pow(0.5);`

result will again be 2km

Or add and substract them if it makes sense, i.e. u cannot add kg to m

> `var distance = new Unit(3, Prefix.k, BaseUnit.m) + new Unit(20, BaseUnit.m);`

distance will be equal to 3020m, as expected.

Compare them if it makes sense

> `bool isGreater = length > distance`

Library recognizes derived units. So if you for example instantiate:

> `var force = new Unit(1, BaseUnit.g) * new Unit(1, BaseUnit.m) * new Unit(1, BaseUnit.s, -2);`

force.ToString() will be "1mN"

You can also use factory method to create Derived units

> `var force = 4 * Unit.Create("N");`