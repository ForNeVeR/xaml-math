module WpfMath.Tests.BrushContainerTests

open System.Windows.Media
open WpfMath.Colors
open WpfMath.Utils
open Xunit

[<Fact>]
let ``The brush is expected to remain in the container``() =
    let expected = Brushes.Green :> Brush
    let actual: SolidColorBrush = downcast BrushContainer.FromBrush(expected).Value
    Assert.Equal(expected, actual)

[<Fact>]
let ``It's expected that the right brush will be created by color``() =
    let color = RgbaColor((byte)255, (byte)254, (byte)253, (byte)252)
    let expected = SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B))
    let actual: SolidColorBrush = downcast BrushContainer.FromColor(color).Value
    Assert.Equal(expected.Color, actual.Color)
