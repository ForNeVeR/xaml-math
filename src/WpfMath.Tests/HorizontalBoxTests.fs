namespace WpfMath.Tests

open System.Windows
open System.Windows.Media

open Foq
open Xunit

open WpfMath
open WpfMath.Boxes
open WpfMath.Rendering

type HorizontalBoxTests() =
    [<Fact>]
    member __.``HorizontalBox rendering calls to RenderElement for each child``() =
        let x = 0.5
        let y = 1.0

        let mockedRenderer = Mock.Of<IElementRenderer>()
        let horizontalBox = HorizontalBox()
        let child1 = HorizontalBox()
        let child2 = HorizontalBox()
        horizontalBox.Add(child1)
        horizontalBox.Add(child2)

        horizontalBox.RenderTo(mockedRenderer, x, y)
        Mock.Verify(<@ mockedRenderer.RenderElement(child1, x, any()) @>, once)
        Mock.Verify(<@ mockedRenderer.RenderElement(child2, x, any()) @>, once)
