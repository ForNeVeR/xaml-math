module WpfMath.Tests.UserDefinedColorParserTests

open Xunit

open WpfMath

[<Fact>]
let ``Hex literal should be validated properly``() =
    let example = "#1122XX33"
    let mutable x = ResizeArray()
    Assert.False(UserDefinedColorParser.IsByteHexTrain(example, 6, ref x))
