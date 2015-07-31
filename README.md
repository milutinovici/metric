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

Now that you have these units you can multiply or divide them

> `var height = volume / area;`

result will be equal to 2km

Power them:

> `var length = area.Pow(0.5);`

result will again be 2km

Or add and substract them if it makes sense, i.e. u cannot add kg to m

> `var distance = Unit.Parse("3km") + Unit.Parse("20m")`

distance will be equal to 3020m, as expected.

Compare them if it makes sense

> `bool isGreater = Unit.Parse("3km") > Unit.Parse("20m")`

Library recognizes derived units. So if you for example instantiate:

> `var force = Unit.Parse("1kg*m*s^-2");`

force.ToString() will be "1N"

You can also use factory method to create Derived units

> `var force = 4 * Unit.Create("N");`