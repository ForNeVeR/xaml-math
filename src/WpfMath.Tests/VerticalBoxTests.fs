namespace WpfMath.Tests


open Foq
open Xunit

open XamlMath.Boxes
open XamlMath.Rendering

type VerticalBoxTests() =
    [<Fact>]
    member _.``VerticalBox rendering calls to RenderElement for each child``() =
        let x = 0.5
        let y = 1.0

        let mockedRenderer = Mock.Of<IElementRenderer>()
        let horizontalBox = HorizontalBox()
        let child1 = HorizontalBox()
        let child2 = HorizontalBox()
        horizontalBox.Add(child1)
        horizontalBox.Add(child2)

        horizontalBox.RenderTo(mockedRenderer, x, y)
        Mock.Verify(<@ mockedRenderer.RenderElement(child1, any(), any()) @>, once)
        Mock.Verify(<@ mockedRenderer.RenderElement(child2, any(), any()) @>, once)
