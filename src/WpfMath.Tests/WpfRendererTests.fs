namespace WpfMath.Tests

open System
open System.Windows
open System.Windows.Media

open Foq
open Xunit

open WpfMath
open WpfMath.Boxes
open WpfMath.Rendering

type WpfRendererTests() =
    static do Utils.initializeFontResourceLoading()

    let group = DrawingGroup()
    let context = group.Open()
    let renderer = WpfElementRenderer(context, 1.0) :> IElementRenderer

    [<Fact>]
    member _.``WpfElementRenderer.RenderElement delegates to element.RenderTo``() : unit =
        let box = Mock.Of<Box>()
        renderer.RenderElement(box, 1.0, 2.0)
        Mock.Verify(<@ box.RenderTo(renderer, 1.0, 2.0) @>, once)

    interface IDisposable with
        member _.Dispose() = (context :> IDisposable).Dispose()
