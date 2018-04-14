module WpfMath.Tests.SourceDetectionTests

open Xunit

open WpfMath
open WpfMath.Tests.Utils

[<Fact>]
let ``2+2 should be parsed properly`` () =
    let source = "2+2"
    let src = src source
    let tree = rowSrc [charSrc '2' (src 0 1); symbolSrc "plus" (src 1 1); charSrc '2' (src 2 1)] (src 0 3)
    assertParseResultWithSource
    <| source
    <| formula tree
