module WpfMath.Tests.SourceDetectionTests

open Xunit

open WpfMath.Tests.ApprovalTestUtils
open WpfMath.Tests.Utils

// TODO[F]: Drop this test (already covered by ParserTests)
[<Fact>]
let ``2+2 should be parsed properly`` () =
    let source = "2+2"
    let src = src source
    verifyParseResult source

// TODO[F]: Merge this into ParserTests
[<Fact>]
let ``integral expression should be parsed properly`` () =
    let source = @"\int_a^b"
    let src = src source
    verifyParseResult source
