module WpfMath.Tests.BoxTests

open System

open FSharp.Core.Fluent
open Xunit

open WpfMath
open WpfMath.Atoms
open WpfMath.Boxes
open WpfMath.Tests.ApprovalTestUtils

let private parse(text: string) =
    let parser = TexFormulaParser()
    let result = parser.Parse text
    result.RootAtom

let private src (string: string) (start: int) (len: int) = SourceSpan("User input", string, start, len)

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

    let baseAtom = CharAtom(src "x" 0 1, 'x')
    let scriptsAtom = ScriptsAtom(null, baseAtom, null, null)

    let box = scriptsAtom.CreateBox(environment)

    let expectedShift = -(box.Height + box.Depth) / 2.0 - environment.MathFont.GetAxisHeight(environment.Style)
    Assert.Equal(expectedShift, box.Shift)

[<Fact>]
let ``RowAtom creates boxes with proper sources``() =
    let source = "2+2"
    let src = src source
    let parser = TexFormulaParser()
    let formula = parser.Parse source
    let box = formula.CreateBox environment :?> HorizontalBox
    let chars = box.Children.filter (fun x -> x :? CharBox)
    Assert.Collection (
        chars,
        Action<_>(fun (x: Box) -> Assert.Equal(src 0 1, x.Source)),
        Action<_>(fun (x: Box) -> Assert.Equal(src 1 1, x.Source)),
        Action<_>(fun (x: Box) -> Assert.Equal(src 2 1, x.Source)))

[<Fact>]
let ``BigOperatorAtom creates a box with proper sources``() =
    let source = @"\int_a^b"
    let src = src source
    let parser = TexFormulaParser()
    let formula = parser.Parse source
    let box = formula.CreateBox environment :?> VerticalBox

    let charBoxes =
        box.Children
            .filter(fun x -> x :? HorizontalBox)
            .collect(fun x -> x.Children.filter (fun y -> y :? CharBox))
            .toList()

    Assert.Collection (
        charBoxes,
        Action<_>(fun (x: Box) -> Assert.Equal(src 7 1, x.Source)),
        Action<_>(fun (x: Box) -> Assert.Equal(src 1 3, x.Source)),
        Action<_>(fun (x: Box) -> Assert.Equal(src 5 1, x.Source)))

[<Fact>]
let ``Cyrillic followed by Latin should be rendered properly``() =
    Utils.initializeFontResourceLoading()
    let atom = parse @"\text{Ц}V"
    let box = atom.CreateBox environment
    Assert.NotNull(box)

let private verifyBox source =
    let atom = parse source
    let box = atom.CreateBox environment
    verifyObject box

[<Fact>]
let simpleMatrixBox() =
    verifyBox @"\pmatrix{2 & 2 \\ 2 & 2}"

[<Fact>]
let casesBox() =
    verifyBox @"\cases{a \\ b \\ c}"

[<Fact>]
let nestedMatrixBox() =
    verifyBox @"\matrix{ 1 & 2 & 3 \\ 4 & {\matrix{ 5 \\ 6 }} & 7 }"

[<Fact>]
let wideItemInMatrixBox() =
    verifyBox @"x = \pmatrix{0 & -r & 0 \\ 0 & 0 & -r sin^2(\theta)}"

[<Fact>]
let shortСommandForThinspace(): unit =
    verifyBox @"\,"

[<Fact>]
let shortСommandForNotEqual(): unit =
    verifyBox @"\neq"

[<Fact>]
let emptyColorbox(): unit =
    verifyBox @"\colorbox{red}{}"

[<Fact>]
let emptyMathrm(): unit =
    verifyBox @"\mathrm{}"

[<Fact>]
let emptyCommandText(): unit =
    verifyBox @"\text{}"

[<Fact>]
let emptyColorRed(): unit =
    verifyBox @"\color{red}{}"
