module WpfMath.Tests.GeometryHelperTests

open System.Windows

open Xunit

open WpfMath.Rendering

[<Fact>]
let ``ScaleRectangle scales the rectangle``() =
    let rect = Rect(1.0, 2.0, 3.0, 4.0)
    let scale = 10.0
    let expected = Rect(10.0, 20.0, 30.0, 40.0)
    Assert.Equal(expected, GeometryHelper.ScaleRectangle(scale, rect))
