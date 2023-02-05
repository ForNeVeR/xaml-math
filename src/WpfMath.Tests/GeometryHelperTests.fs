module WpfMath.Tests.GeometryHelperTests

open Xunit

open XamlMath.Rendering

[<Fact>]
let ``ScaleRectangle scales the rectangle``() =
    let rect = Rectangle(1.0, 2.0, 3.0, 4.0)
    let scale = 10.0
    let expected = Rectangle(10.0, 20.0, 30.0, 40.0)
    Assert.Equal(expected, GeometryHelper.ScaleRectangle(scale, rect))
