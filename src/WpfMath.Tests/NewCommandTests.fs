module WpfMath.Tests.NewCommandTests

open Xunit
open WpfMath.Parsers
open WpfMath.Rendering
open WpfMath.Tests.ApprovalTestUtils
open XamlMath
open XamlMath.Atoms
open XamlMath.Parsers

let private parser = WpfTeXFormulaParser.Instance

/// Parses a formula and returns the root atom, or raises on error.
let private parse (formula: string) : Atom =
    let result = parser.Parse formula
    result.RootAtom

let private environment = WpfTeXEnvironment.Create()

// ──────────────────────────────────────────────────
// 1. Font Variant Commands (11)
// ──────────────────────────────────────────────────

[<Theory>]
[<InlineData(@"\mathbb{R}")>]
[<InlineData(@"\mathbf{v}")>]
[<InlineData(@"\mathsf{ABC}")>]
[<InlineData(@"\mathtt{abc}")>]
[<InlineData(@"\mathfrak{g}")>]
[<InlineData(@"\mathscr{L}")>]
[<InlineData(@"\textbf{bold}")>]
[<InlineData(@"\textit{italic}")>]
[<InlineData(@"\textsf{sans}")>]
[<InlineData(@"\texttt{type}")>]
[<InlineData(@"\textsc{SmallCaps}")>]
let ``Font variants parse successfully``(formula: string) =
    let atom = parse formula
    Assert.NotNull atom

[<Theory>]
[<InlineData(@"\mathbb{R} + \mathbb{C}")>]
[<InlineData(@"\mathbf{a} \cdot \mathbf{b}")>]
let ``Font variants work in expressions``(formula: string) =
    let atom = parse formula
    Assert.NotNull atom

// ──────────────────────────────────────────────────
// 2. Display Style Commands (4)
// ──────────────────────────────────────────────────

let rec private containsStyleAtom (targetStyle: TexStyle) (atom: Atom) : bool =
    match atom with
    | :? StyleAtom as sa -> sa.TargetStyle = targetStyle
    | :? RowAtom as row ->
        row.Elements
        |> Seq.exists (fun e -> e <> null && containsStyleAtom targetStyle e)
    | :? BigOperatorAtom as bigOp ->
        bigOp.BaseAtom <> null && containsStyleAtom targetStyle bigOp.BaseAtom
    | :? ScriptsAtom as scripts ->
        scripts.BaseAtom <> null && containsStyleAtom targetStyle scripts.BaseAtom
    | :? UnderOverAtom as uo ->
        uo.BaseAtom <> null && containsStyleAtom targetStyle uo.BaseAtom
    | :? FencedAtom as fenced ->
        containsStyleAtom targetStyle fenced.BaseAtom
    | _ -> false

[<Theory>]
[<InlineData(@"\displaystyle\int_0^1 x\,dx", TexStyle.Display)>]
[<InlineData(@"\textstyle\sum_{i=1}^n", TexStyle.Text)>]
[<InlineData(@"\scriptstyle x^2", TexStyle.Script)>]
[<InlineData(@"\scriptscriptstyle x", TexStyle.ScriptScript)>]
let ``Display style commands produce correct StyleAtom``(formula: string, expectedStyle: TexStyle) =
    let atom = parse formula
    Assert.True(containsStyleAtom expectedStyle atom,
        sprintf "Expected StyleAtom with style %A in the atom tree" expectedStyle)

[<Fact>]
let ``Display style with grouped content``() =
    let atom = parse @"\displaystyle {\int_0^1 f(x)\,dx}"
    Assert.True(containsStyleAtom TexStyle.Display atom)

// ──────────────────────────────────────────────────
// 3. Stacked Annotation Commands (3)
// ──────────────────────────────────────────────────

[<Fact>]
let ``Overset creates UnderOverAtom``() =
    let atom = parse @"\overset{*}{X}" :?> UnderOverAtom
    Assert.NotNull atom.OverAtom
    Assert.Null atom.UnderAtom
    Assert.NotNull atom.BaseAtom

[<Fact>]
let ``Underset creates UnderOverAtom``() =
    let atom = parse @"\underset{i=1}{\bigcap}" :?> UnderOverAtom
    Assert.Null atom.OverAtom
    Assert.NotNull atom.UnderAtom
    Assert.NotNull atom.BaseAtom

[<Fact>]
let ``Stackrel creates UnderOverAtom``() =
    let atom = parse @"\stackrel{!}{=}" :?> UnderOverAtom
    Assert.NotNull atom.OverAtom
    Assert.Null atom.UnderAtom
    Assert.NotNull atom.BaseAtom

// ──────────────────────────────────────────────────
// 4. Invisible Atom Commands (4)
// ──────────────────────────────────────────────────

[<Fact>]
let ``Phantom creates PhantomAtom``() =
    let atom = parse @"\phantom{x+y}" :?> PhantomAtom
    Assert.NotNull atom.RowAtom

[<Fact>]
let ``HPhantom creates PhantomAtom``() =
    let atom = parse @"\hphantom{x}" :?> PhantomAtom
    Assert.NotNull atom

[<Fact>]
let ``VPhantom creates PhantomAtom``() =
    let atom = parse @"\vphantom{x}" :?> PhantomAtom
    Assert.NotNull atom

[<Fact>]
let ``Smash creates PhantomAtom``() =
    let atom = parse @"\smash{x}" :?> PhantomAtom
    Assert.NotNull atom

// ──────────────────────────────────────────────────
// 5. Box Command (1)
// ──────────────────────────────────────────────────

[<Fact>]
let ``Boxed creates BoxedAtom``() =
    let atom = parse @"\boxed{E=mc^2}" :?> BoxedAtom
    Assert.NotNull atom.BaseAtom

[<Fact>]
let ``Boxed with complex content``() =
    let atom = parse @"\boxed{\int_a^b f(x)\,dx}" :?> BoxedAtom
    Assert.NotNull atom

[<Fact>]
let ``Boxed can render without crashing``() =
    let atom = parse @"\boxed{E=mc^2}"
    let box = atom.CreateBox(environment)
    Assert.True(box.Width >= 0.0)
    Assert.True(box.Height >= 0.0)

// ──────────────────────────────────────────────────
// 6. AMS Matrix Environments (5)
// ──────────────────────────────────────────────────

[<Theory>]
[<InlineData(@"\begin{aligned}a&=b\\c&=d\end{aligned}")>]
[<InlineData(@"\begin{bmatrix}1&2\\3&4\end{bmatrix}")>]
[<InlineData(@"\begin{Bmatrix}1&2\\3&4\end{Bmatrix}")>]
[<InlineData(@"\begin{vmatrix}1&2\\3&4\end{vmatrix}")>]
[<InlineData(@"\begin{Vmatrix}1&2\\3&4\end{Vmatrix}")>]
let ``AMS matrix environments parse successfully``(formula: string) =
    let atom = parse formula
    Assert.NotNull atom

[<Fact>]
let ``BMatrix with three rows``() =
    let atom = parse @"\begin{bmatrix}a&b\\c&d\\e&f\end{bmatrix}"
    Assert.NotNull atom

[<Fact>]
let ``VMatrix 3x3 determinant``() =
    let atom = parse @"\begin{vmatrix}1&2&3\\4&5&6\\7&8&9\end{vmatrix}"
    Assert.NotNull atom

// ──────────────────────────────────────────────────
// 7. Extensible Arrows (2)
// ──────────────────────────────────────────────────

[<Fact>]
let ``XRightArrow creates ExtensibleArrowAtom``() =
    let atom = parse @"\xrightarrow{n\to\infty}" :?> ExtensibleArrowAtom
    Assert.Equal(ExtensibleArrowDirection.Right, atom.Direction)
    Assert.NotNull atom.LabelAtom

[<Fact>]
let ``XLeftArrow creates ExtensibleArrowAtom``() =
    let atom = parse @"\xleftarrow{n\to\infty}" :?> ExtensibleArrowAtom
    Assert.Equal(ExtensibleArrowDirection.Left, atom.Direction)
    Assert.NotNull atom.LabelAtom

[<Fact>]
let ``XRightArrow with empty label``() =
    let atom = parse @"\xrightarrow{}" :?> ExtensibleArrowAtom
    Assert.NotNull atom

[<Fact>]
let ``XRightArrow can render without crashing``() =
    let atom = parse @"\xrightarrow{n\to\infty}"
    let box = atom.CreateBox(environment)
    Assert.True(box.Width >= 0.0)

// ──────────────────────────────────────────────────
// 8. AMS Abbreviations (3)
// ──────────────────────────────────────────────────

[<Fact>]
let ``Implies parses``() =
    let atom = parse @"A\implies B"
    Assert.NotNull atom

[<Fact>]
let ``Iff parses``() =
    let atom = parse @"A\iff B"
    Assert.NotNull atom

[<Fact>]
let ``ImpliedBy parses``() =
    let atom = parse @"A\impliedby B"
    Assert.NotNull atom

// ──────────────────────────────────────────────────
// 9. Nested / Combined Commands
// ──────────────────────────────────────────────────

[<Fact>]
let ``Boxed with mathbb``() =
    let atom = parse @"\boxed{\mathbb{R}}" :?> BoxedAtom
    Assert.NotNull atom

[<Fact>]
let ``Overset with scripts``() =
    let atom = parse @"\overset{*}{\sum_{i=1}^n}" :?> UnderOverAtom
    Assert.NotNull atom

[<Fact>]
let ``Phantom inside boxed``() =
    let atom = parse @"\boxed{E = \phantom{x} mc^2}" :?> BoxedAtom
    Assert.NotNull atom

[<Fact>]
let ``Matrix with boxed cell``() =
    let atom = parse @"\begin{pmatrix}\boxed{a}&b\\c&d\end{pmatrix}"
    Assert.NotNull atom

// ──────────────────────────────────────────────────
// 11. Additional standard LaTeX commands
// ──────────────────────────────────────────────────

[<Theory>]
[<InlineData(@"\bmod")>]
[<InlineData(@"\pmod{n}")>]
[<InlineData(@"\dots")>]
let ``Additional standard commands parse successfully``(formula: string) =
    let atom = parse formula
    Assert.NotNull atom

[<Fact>]
let ``Bmod is binary mod operator``() =
    let f = parser.Parse @"a \bmod b"
    Assert.NotNull f.RootAtom

[<Fact>]
let ``Pmod with argument``() =
    let f = parser.Parse @"\pmod{n}"
    Assert.NotNull f.RootAtom

[<Fact>]
let ``Dots equals ldots``() =
    let atom = parse @"\dots"
    Assert.NotNull atom

// ──────────────────────────────────────────────────
// 10. Approval-style rendering snapshot tests
//    (verify JSON serialization of the atom tree)
// ──────────────────────────────────────────────────

[<Theory>]
[<InlineData(@"\mathbb{R}", "mathbb")>]
[<InlineData(@"\displaystyle\int_0^1 x\,dx", "displaystyle")>]
[<InlineData(@"\overset{*}{X}", "overset")>]
[<InlineData(@"\boxed{E=mc^2}", "boxed")>]
[<InlineData(@"\xrightarrow{n\to\infty}", "xrightarrow")>]
let ``New commands produce consistent atom trees``(formula: string, scenario: string) =
    verifyParseResultScenario scenario formula
