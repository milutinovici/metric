# SI Measurement Units #

**MeasurementUnits** is a .Net library written in C#, designed to help developers who are dealing with measurement units. 

## Usage ##

Want to instantiate  8km^3? You can do it like so:
> `var u1 = new Unit(8, Prefix.k, BaseUnit.m, 3);`

Or you can parse it from a string:
> `var u2 = Unit.Parse("4km^2");`

Now that you have these units you can multiply or divide them

> `var u3 = u1 / u2;`

result will be equal to 2km

Power them:

`var u4 = u2.Pow(0.5);`

result will again be 2km

Or add and substract them if it makes sense, i.e. u cannot add kg to m

> `var u5 = Unit.Parse("3km") + Unit.Parse("20m")`

u5 will be equal to 3020m, as expected.

Compare them if it makes sense

> `bool isGreater = Unit.Parse("3km") > Unit.Parse("20m")`

Library recognizes derived units. So if you for example instantiate:

> `var u6 = Unit.Parse("1kg*m*s^-2");`

u5.ToString() will be 1N

You can also use factory method to create Derived units

> `var u7 = 4 * Unit.Create("N");`