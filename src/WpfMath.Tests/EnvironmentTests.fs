module WpfMath.Tests.EnvironmentTests

open Xunit

open WpfMath.Exceptions
open WpfMath.Tests.ApprovalTestUtils
open WpfMath.Tests.Utils

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
