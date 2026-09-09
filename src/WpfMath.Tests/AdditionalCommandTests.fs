module WpfMath.Tests.AdditionalCommandTests

open Xunit
open WpfMath.Parsers
open WpfMath.Rendering
open XamlMath
open XamlMath.Atoms
open XamlMath.Parsers

let private parser = WpfTeXFormulaParser.Instance

let private parse (formula: string) : Atom =
    let result = parser.Parse formula
    result.RootAtom

let private environment = WpfTeXEnvironment.Create()

// ──────────────────────────────────────────────────
// 1. \textcolor alias for \color
// ──────────────────────────────────────────────────

[<Fact>]
let ``TextColor parses like Color``() =
    let atom = parse @"\textcolor{red}{x}"
    Assert.NotNull atom

[<Fact>]
let ``TextColor with RGB model``() =
    let atom = parse @"\textcolor[RGB]{255,0,0}{text}"
    Assert.NotNull atom

// ──────────────────────────────────────────────────
// 2. More extensible arrows
// ──────────────────────────────────────────────────

[<Fact>]
let ``XMapsto parses``() =
    let atom = parse @"\xmapsto{a}" :?> ExtensibleArrowAtom
    Assert.NotNull atom
    Assert.Equal(ExtensibleArrowDirection.Right, atom.Direction)

[<Fact>]
let ``XHookrightarrow parses``() =
    let atom = parse @"\xhookrightarrow{a}" :?> ExtensibleArrowAtom
    Assert.NotNull atom

[<Fact>]
let ``XRightarrow parses``() =
    let atom = parse @"\xRightarrow{a}" :?> ExtensibleArrowAtom
    Assert.NotNull atom

[<Fact>]
let ``XLeftarrow parses``() =
    let atom = parse @"\xLeftarrow{a}" :?> ExtensibleArrowAtom
    Assert.Equal(ExtensibleArrowDirection.Left, atom.Direction)

[<Fact>]
let ``XHookleftarrow parses``() =
    let atom = parse @"\xhookleftarrow{a}" :?> ExtensibleArrowAtom
    Assert.NotNull atom

// ──────────────────────────────────────────────────
// 3. \gathered environment
// ──────────────────────────────────────────────────

[<Fact>]
let ``Gathered environment parses``() =
    let f = parser.Parse @"\begin{gathered}a=b\\c=d\end{gathered}"
    Assert.NotNull f.RootAtom

// ──────────────────────────────────────────────────
// 4. \smallmatrix environment (inline)
// ──────────────────────────────────────────────────

[<Fact>]
let ``SmallMatrix parses``() =
    let f = parser.Parse @"\begin{smallmatrix}a&b\\c&d\end{smallmatrix}"
    Assert.NotNull f.RootAtom

// ──────────────────────────────────────────────────
// 5. \intertext
// ──────────────────────────────────────────────────

[<Fact>]
let ``Intertext parses in align``() =
    let f = parser.Parse @"\begin{align}a&=b\\\intertext{text}c&=d\end{align}"
    Assert.NotNull f.RootAtom

// ──────────────────────────────────────────────────
// 6. \mathclap / \mathrlap / \mathllap
// ──────────────────────────────────────────────────

[<Fact>]
let ``MathClap parses``() =
    let atom = parse @"\mathclap{x}"
    Assert.NotNull atom

[<Fact>]
let ``MathRlap parses``() =
    let atom = parse @"\mathrlap{x}"
    Assert.NotNull atom

[<Fact>]
let ``MathLlap parses``() =
    let atom = parse @"\mathllap{x}"
    Assert.NotNull atom

// ──────────────────────────────────────────────────
// 7. \left. and \right. (empty delimiters)
// ──────────────────────────────────────────────────

[<Fact>]
let ``Empty left delimiter``() =
    let atom = parse @"\left.a\right)"
    Assert.NotNull atom

[<Fact>]
let ``Empty right delimiter``() =
    let atom = parse @"\left(\right."
    Assert.NotNull atom

[<Fact>]
let ``Both empty delimiters``() =
    let atom = parse @"\left.\right."
    Assert.NotNull atom

// ──────────────────────────────────────────────────
// 8. Rendering smoke tests
// ──────────────────────────────────────────────────

[<Theory>]
[<InlineData(@"\textcolor{red}{x}")>]
[<InlineData(@"\xmapsto{a}")>]
[<InlineData(@"\begin{gathered}a=b\\c=d\end{gathered}")>]
[<InlineData(@"\mathclap{x}")>]
[<InlineData(@"\left.\right)")>]
let ``New commands render without crashing``(formula: string) =
    let atom = parse formula
    let box = atom.CreateBox(environment)
    Assert.True(box.Width >= 0.0)
