module WpfMath.Tests.SourceDetectionTests

open Xunit

open WpfMath
open WpfMath.Tests.Utils

[<Fact>]
let ``2+2 should be parsed properly`` () =
    let source = "2+2"
    let src = src source
    let tree = rowSrc [ charSrc '2' (src 0 1)
                        symbolSrc "plus" TexAtomType.BinaryOperator (src 1 1)
                        charSrc '2' (src 2 1) ]
                      (src 0 3)
    assertParseResultWithSource
    <| source
    <| formula tree

[<Fact>]
let ``integral expression should be parsed properly`` () =
    let source = @"\int_a^b"
    let src = src source
    let tree = opWithScriptsSrc (symbolSrc "int" TexAtomType.BigOperator (src 1 3))
                                (charSrc 'a' (src 5 1))
                                (charSrc 'b' (src 7 1))
                                None
                                (src 0 8)
    assertParseResultWithSource
    <| source
    <| formula tree
