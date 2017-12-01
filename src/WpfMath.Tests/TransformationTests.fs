module WpfMath.Tests.TransformationTests

open System.Windows
open System.Windows.Media

open Xunit

open WpfMath.Rendering.Transformations

[<Fact>]
let ``Translate transformation scales properly``() =
    let transformation = Transformation.Translate(1.0, 2.0)
    let scale = 10.0
    let expected = Transformation.Translate(10.0, 20.0)
    let scaled = transformation.Scale scale :?> Transformation.Translate
    Assert.Equal(expected.X, scaled.X)
    Assert.Equal(expected.Y, scaled.Y)

[<Fact>]
let ``Rotate transformation doesn't scale``() =
    let transformation = Transformation.Rotate(90.0)
    let scale = 10.0
    Assert.Equal(transformation, downcast transformation.Scale scale)
