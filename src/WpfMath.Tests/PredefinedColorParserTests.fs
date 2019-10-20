module WpfMath.Tests.PredefinedColorParserTests

open System.Windows.Media

open Xunit

open WpfMath.Colors

let parser = PredefinedColorParser.Instance

[<Fact>]
let ``PredefinedColorParser parses a correctly defined color``(): unit =
    Assert.Equal(Color.FromRgb(237uy, 27uy, 35uy), parser.Parse([| "red" |]).Value)

[<Fact>]
let ``PredefinedColorParser returns null for wrong input``(): unit =
    Assert.Null(parser.Parse([| "nonexistent-color" |]))

[<Fact>]
let ``PredefinedColorParser returns null for empty input``(): unit =
    Assert.Null(parser.Parse(Array.empty))

[<Fact>]
let ``PredefinedColorParser returns null for too long input``(): unit =
    Assert.Null(parser.Parse([| "red"; "green" |]))
