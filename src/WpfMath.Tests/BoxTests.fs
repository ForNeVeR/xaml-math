module WpfMath.Tests.BoxTests

open DeepEqual.Syntax
open Xunit

open WpfMath
open WpfMath.Exceptions
open WpfMath.Tests.Utils

let private parse text =
    let parser = TexFormulaParser()
    let result = parser.Parse text
    result.RootAtom

let private environment =
    let mathFont = DefaultTexFont 20.0
    let textFont = TexFormula.GetSystemFont("Arial", 20.0)
    TexEnvironment(TexStyle.Display, mathFont, textFont)

[<Fact>]
let ``AccentedAtom should have a skew according to the char``() =
    let topAtom = parse @"\bar{\bar{x}}" :?> AccentedAtom
    let childAtom = topAtom.BaseAtom :?> AccentedAtom

    let topBox = topAtom.CreateBox(environment).Children.[0]
    let childBox = childAtom.CreateBox(environment).Children.[0]

    Assert.Equal(topBox.Shift, childBox.Shift)

[<Fact>]
let ``Box for \text{æ,} should be created successfully``() =
    let atom = parse @"\text{æ,}"
    let box = atom.CreateBox(environment)
    Assert.NotNull(box)

[<Fact>]
let ``ScriptsAtom should set Shift on the created box when creating box without any sub- or superscript``() =
    Utils.initializeFontResourceLoading()

    let baseAtom = CharAtom('x')
    let scriptsAtom = ScriptsAtom(baseAtom, null, null)

    let box = scriptsAtom.CreateBox(environment)

    let expectedShift = -(box.Height + box.Depth) / 2.0 - environment.MathFont.GetAxisHeight(environment.Style)
    Assert.Equal(expectedShift, box.Shift)
