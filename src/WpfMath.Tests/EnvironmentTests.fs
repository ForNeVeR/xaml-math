module WpfMath.Tests.EnvironmentTests

open Xunit

open WpfMath.Tests.ApprovalTestUtils
open WpfMath.Tests.Utils
open XamlMath.Exceptions

[<Fact>]
let pMatrixEnvironment(): unit =
    verifyParseResult @"\begin{pmatrix}{line 1}\\line 2\end{pmatrix}"

[<Fact>]
let nestedEnvironment(): unit =
    verifyParseResult @"\begin{pmatrix}line 1\\\begin{pmatrix}line x\end{pmatrix}\end{pmatrix}"

[<Fact>]
let nestedMatrix(): unit =
    verifyParseResult @"\begin{pmatrix}line 1\\\pmatrix{line x & line y}\end{pmatrix}"

[<Fact>]
let ``Empty environment name should trigger an exception``(): unit =
    let markup = @"\begin{}"
    let ex = assertParseThrows<TexParseException> markup
    Assert.Equal(@"Empty environment name for the \begin command.", ex.Message)

[<Fact>]
let ``Unknown environment should trigger an exception``(): unit =
    let markup = @"\begin{unknown}"
    let ex = assertParseThrows<TexParseException> markup
    Assert.Equal(@"Unknown environment name for the \begin command: ""unknown"".", ex.Message)

[<Fact>]
let ``Broken nested environments should throw an exception``(): unit =
    let markup = @"\begin{pmatrix}\begin{pmatrix}\end{pmatrix}"
    let ex = assertParseThrows<TexParseException> markup
    Assert.Equal(@"No matching \end found for command ""\begin{pmatrix}"".", ex.Message)

[<Fact>]
let ``Not corresponding \end should throw an exception``(): unit =
    let markup = @"\begin{pmatrix}\end{unknown}"
    let ex = assertParseThrows<TexParseException> markup
    Assert.Equal(@"""\end{unknown}"" doesn't correspond to earlier ""\begin{pmatrix}"".", ex.Message)
