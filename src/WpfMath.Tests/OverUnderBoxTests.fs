namespace WpfMath.Tests

open System.Windows
open System.Windows.Media

open Foq
open Xunit

open WpfMath
open WpfMath.Boxes
open WpfMath.Rendering

type OverUnderBoxTests() =
    let x = 0.5
    let y = 1.0

    let mockedRenderer = Mock.Of<IElementRenderer>()

    let baseBox = HorizontalBox()
    let delimiterBox = HorizontalBox()
    let scriptBox = HorizontalBox()
    let overUnderBox = OverUnderBox(baseBox, delimiterBox, scriptBox, 1.0, true)

    [<Fact>]
    member __.``OverUnderBox rendering calls to RenderElement for base and script``() =
        overUnderBox.RenderTo(mockedRenderer, x, y)
        Mock.Verify(<@ mockedRenderer.RenderElement(baseBox, x, any()) @>, once)
        Mock.Verify(<@ mockedRenderer.RenderTransformed(delimiterBox, any(), any(), any()) @>, once)
        Mock.Verify(<@ mockedRenderer.RenderElement(scriptBox, x, any()) @>, once)

    [<Fact>]
    member __.``OverUnderBox rendering calls to RenderTransformed for delimiter``() =
        overUnderBox.RenderTo(mockedRenderer, x, y)
        Mock.Verify(<@ mockedRenderer.RenderTransformed(delimiterBox, any(), any(), any()) @>, once)
