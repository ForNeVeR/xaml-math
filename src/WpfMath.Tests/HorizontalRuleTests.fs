namespace WpfMath.Tests

open System.Windows
open System.Windows.Media

open Foq
open Xunit

open WpfMath
open WpfMath.Boxes
open WpfMath.Rendering

type HorizontalRuleTests() =
    static do Utils.initializeFontResourceLoading()

    [<Fact>]
    member __.``HorizontalRule rendering calls to RenderRect``() =
        let font = DefaultTexFont 20.0
        let environment = TexEnvironment(TexStyle.Display, font, font)
        let x = 0.5
        let y = 1.0
        let thickness = 2.0
        let width = 3.0
        let shift = 4.0

        let mockedRenderer = Mock.Of<IElementRenderer>()
        let horizontalRule = HorizontalRule(environment, thickness, width, shift)
        horizontalRule.RenderTo(mockedRenderer, x, y)

        let expectedRect = Rect(x, y -  thickness, width, thickness)
        Mock.Verify(<@ mockedRenderer.RenderRectangle(expectedRect, Brushes.Black) @>, once)
