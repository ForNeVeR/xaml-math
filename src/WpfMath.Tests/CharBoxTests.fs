namespace WpfMath.Tests

open System.Windows
open System.Windows.Media

open Foq
open Xunit

open WpfMath
open WpfMath.Boxes
open WpfMath.Rendering
open WpfMath.Exceptions
open System

type CharBoxTests() =
    static do Utils.initializeFontResourceLoading()

    let parse(text: string) =
        let parser = TexFormulaParser()
        let result = parser.Parse text
        result.RootAtom

    let environment =
        let mathFont = DefaultTexFont 20.0
        let textFont = TexFormula.GetSystemFont("Arial", 20.0)
        TexEnvironment(TexStyle.Display, mathFont, textFont)

    [<Fact>]
    member _.``CharBox rendering calls to RenderGlyphRun``() =
        let char = environment.MathFont.GetDefaultCharInfo('x', TexStyle.Display).Value
        let x = 0.5
        let y = 1.0

        let mockedRenderer = Mock.Of<IElementRenderer>()
        let charBox = CharBox(environment, char)
        charBox.RenderTo(mockedRenderer, x, y)
        Mock.Verify(<@ mockedRenderer.RenderGlyphRun(any(), x, y, Brushes.Black) @>, once)

    [<Fact>]
    member _.``Currently unsupporteded characters like "Å" should result in TexCharacterMappingNotFoundException``() =
        Assert.IsType<TexCharacterMappingNotFoundException>(
            environment.MathFont.GetDefaultCharInfo('Å', TexStyle.Display).Error)

    [<Fact>]
    member _.``CharBox GetGlyphRun for \text{∅} should throw the TexCharacterMappingNotFoundException``() =
        let atom = parse @"\text{∅}"
        let charBox : CharBox = downcast atom.CreateBox(environment)
        let action = Func<obj>(fun () -> upcast charBox.GetGlyphRun(20.0, 0.5, 1.0))
        let exc = Assert.Throws<TexCharacterMappingNotFoundException>(action)
        Assert.Equal("The Arial font does not support '∅' (U+2205) character.", exc.Message)
