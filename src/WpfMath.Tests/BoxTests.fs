namespace WpfMath.Tests

open DeepEqual.Syntax
open Xunit

open WpfMath
open WpfMath.Exceptions
open WpfMath.Tests.Utils

type BoxTests() =
    [<Fact>]
    member __.``AccentedAtom should have a skew according to the char``() =
        let parser = TexFormulaParser()
        let result = parser.Parse @"\bar{\bar{x}}"
        let topAtom = result.RootAtom :?> AccentedAtom
        let childAtom = topAtom.BaseAtom :?> AccentedAtom

        let font = DefaultTexFont 20.0
        let environment = TexEnvironment(TexStyle.Display, font, font)
        let topBox = topAtom.CreateBox(environment).Children.[0]
        let childBox = childAtom.CreateBox(environment).Children.[0]

        Assert.Equal(topBox.Shift, childBox.Shift)
