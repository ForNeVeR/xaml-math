module WpfMath.Tests.GeometryHelperTests

open System.Windows
open System.Windows.Media

open Xunit

open WpfMath.Rendering

[<Fact>]
let ``ScaleRectangle scales the rectangle``() =
    let rect = Rect(1.0, 2.0, 3.0, 4.0)
    let scale = 10.0
    let expected = Rect(10.0, 20.0, 30.0, 40.0)
    Assert.Equal(expected, GeometryHelper.ScaleRectangle(scale, rect))

[<Fact>]
let ``ScaleTransform scales the TranslateTransform``() =
    let transform = TranslateTransform(1.0, 2.0)
    let scale = 10.0
    let expected = TranslateTransform(10.0, 20.0)
    let scaled = GeometryHelper.ScaleTransform(scale, transform) :?> TranslateTransform
    Assert.Equal(expected.X, scaled.X)
    Assert.Equal(expected.Y, scaled.Y)

[<Fact>]
let ``ScaleTransform doesn't change the RotateTransform``() =
    let transform = RotateTransform(90.0)
    let scale = 10.0
    Assert.Equal(transform, downcast GeometryHelper.ScaleTransform(scale, transform))
