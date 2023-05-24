namespace WpfMath.Tests

open System.Windows.Media

open Foq
open Xunit

open WpfMath.Fonts
open WpfMath.Rendering
open XamlMath
open XamlMath.Boxes
open XamlMath.Rendering
open XamlMath.Rendering.Transformations

type GeometryElementRendererTests() =
    static do Utils.initializeFontResourceLoading()

    let geometry = GeometryGroup()
    let renderer = GeometryElementRenderer(geometry, 1.0) :> IElementRenderer

    [<Fact>]
    member _.``GeometryElementRenderer.RenderElement delegates to element.RenderTo``() : unit =
        let box = Mock.Of<Box>()
        renderer.RenderElement(box, 1.0, 2.0)
        Mock.Verify(<@ box.RenderTo(renderer, 1.0, 2.0) @>, once)

    [<Fact>]
    member _.``GeometryElementRenderer.RenderCharacter adds a PathGeometry group``() : unit =
        let font = DefaultTexFont(WpfMathFontProvider.Instance, 20.0)
        let environment = TexEnvironment(TexStyle.Display, font, font)
        let char = environment.MathFont.GetDefaultCharInfo('x', TexStyle.Display).Value
        renderer.RenderCharacter(char, 0.0, 0.0, WpfExtensions.ToPlatform Brushes.Black)

        let group = Seq.exactlyOne geometry.Children :?> GeometryGroup
        Assert.IsType<PathGeometry>(Seq.exactlyOne group.Children) |> ignore

    [<Fact>]
    member _.``GeometryElementRenderer.RenderRectangle adds a RectangleGeometry``() : unit =
        let rect = Rectangle(1.0, 2.0, 3.0, 4.0)
        renderer.RenderRectangle(rect, null)

        Assert.IsType<RectangleGeometry>(Seq.exactlyOne geometry.Children) |> ignore

    [<Fact>]
    member _.``GeometryElementRenderer.RenderTransformed adds a GeometryGroup``() : unit =
        renderer.RenderTransformed(HorizontalBox(), [| Transformation.Translate(1.0, 1.0) |], 0.0, 0.0)
        Assert.IsType<GeometryGroup>(Seq.exactlyOne geometry.Children) |> ignore
