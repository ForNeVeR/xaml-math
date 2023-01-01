namespace WpfMath.Tests

open System
open System.Windows.Media

open Foq
open Xunit

open WpfMath
open WpfMath.Boxes
open WpfMath.Fonts
open WpfMath.Rendering
open WpfMath.Exceptions

type CharBoxTests() =
    static do Utils.initializeFontResourceLoading()

    let parse(text: string) =
        let parser = TexFormulaParser()
        let result = parser.Parse text
        result.RootAtom

    let environment =
        let mathFont = DefaultTexFont(WpfMathFontProvider.Instance, 20.0)
        let textFont = TexFormula.GetSystemFont("Arial", 20.0)
        TexEnvironment(TexStyle.Display, mathFont, textFont)

    [<Fact>]
    member _.``CharBox rendering calls to RenderCharacter``() =
        let char = environment.MathFont.GetDefaultCharInfo('x', TexStyle.Display).Value
        let x = 0.5
        let y = 1.0

        let mockedRenderer = Mock.Of<IElementRenderer>()
        let charBox = CharBox(environment, char)
        charBox.RenderTo(mockedRenderer, x, y)
        Mock.Verify(<@ mockedRenderer.RenderCharacter(any(), x, y, Brushes.Black) @>, once)

    [<Fact>]
    member _.``Currently unsupported characters like "Å" should result in TexCharacterMappingNotFoundException``() =
        Assert.IsType<TexCharacterMappingNotFoundException>(
            environment.MathFont.GetDefaultCharInfo('Å', TexStyle.Display).Error)

    [<Fact>]
    member _.``CharBox GetGlyphRun for \text{∅} should throw the TexCharacterMappingNotFoundException``() =
        let atom = parse @"\text{∅}"
        let charBox : CharBox = downcast atom.CreateBox(environment)
        let action = Func<obj>(fun () -> upcast WpfCharInfoEx.GetGlyphRun(charBox.Character, 20.0, 0.5, 1.0))
        let exc = Assert.Throws<TexCharacterMappingNotFoundException>(action)
        Assert.Equal("The Arial font does not support '∅' (U+2205) character.", exc.Message)
