namespace WpfMath.Tests

open System.Windows
open System.Windows.Media

open Foq
open Xunit

open WpfMath
open WpfMath.Rendering
open WpfMath.Exceptions
open System

type CharBoxTests() =
    static do Utils.initializeFontResourceLoading()

    [<Fact>]
    member __.``CharBox rendering calls to RenderGlyphRun``() =
        let font = DefaultTexFont 20.0
        let environment = TexEnvironment(TexStyle.Display, font, font)
        let char = environment.MathFont.GetDefaultCharInfo('x', TexStyle.Display)
        let x = 0.5
        let y = 1.0

        let mockedRenderer = Mock.Of<IElementRenderer>()
        let charBox = CharBox(environment, char)
        charBox.RenderTo(mockedRenderer, x, y)
        Mock.Verify(<@ mockedRenderer.RenderGlyphRun(any(), x, y, Brushes.Black) @>, once)

    [<Fact>]
    member __.``Currently unsupporteded characters like "Å" should throw TexCharacterMappingNotFoundException``() =
        let font = DefaultTexFont 20.0
        let environment = TexEnvironment(TexStyle.Display, font, font)
        Assert.Throws<TexCharacterMappingNotFoundException>(Func<obj>(fun () -> upcast environment.MathFont.GetDefaultCharInfo('Å', TexStyle.Display)))
