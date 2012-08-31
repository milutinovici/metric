# SI Measurement Units #

**MeasurementUnits** is a .Net library written in C#, designed to help developers who are dealing with measurement units. 

## Usage ##

Want to instantiate  4kg^2? You can do it like so:
> `var u1 = new new Unit(4, Prefix.k, BaseUnit.g, 2);`

Or you can parse it from a string:
> `var u1 = Unit.Parse("4kg^2");`

You want 2kg*s ?
> `var u2 = new new ComplexUnit(2, new Unit(Prefix.k, BaseUnit.g, 2), new Unit(BaseUnit.s));`

Or just:
> `var u2 = Unit.Parse("2kg*s");`

Now that you have these units you can multiply or divide them

> `var u3 = u1 / u2;`

u3 will be equal to 2kg*s^-1

Or add and substract them if it makes sense, i.e. u cannot add kg to m

> `var u4 = Unit.Parse("3km") + Unit.Parse("20m")`

u4 will be equal to 3020m, as expected.

Library recognizes derived units. So if you for example instantiate:

> `var u5 = Unit.Parse("1kg*m*s^-2");`

u5.ToString() will be 1N