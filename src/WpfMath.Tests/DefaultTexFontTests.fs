namespace WpfMath.Tests

open Xunit

open WpfMath
open WpfMath.Exceptions

type DefaultTexFontTests() =
    static do Utils.initializeFontResourceLoading()

    let font = DefaultTexFont(1.0)

    [<Fact>]
    member _.``GetCharInfo(char, string, TexStyle) returns a CharInfo for existing character``() =
        Assert.NotNull <| font.GetCharInfo('x', "text", TexStyle.Text)

    [<Fact>]
    member _.``GetCharInfo(char, string, TexStyle) throws a TextStyleMappingNotFoundException for unknown text style``() =
        Assert.IsType<TextStyleMappingNotFoundException>(font.GetCharInfo('x', "unknownStyle", TexStyle.Text).Error)

    [<Fact>]
    member _.``GetCharInfo(string, TexStyle) returns a CharInfo for existing symbol``() =
        Assert.NotNull <| font.GetCharInfo("sqrt", TexStyle.Text)

    [<Fact>]
    member _.``GetCharInfo(string, TexStyle) throws a SymbolMappingNotFoundException for unknown symbol``() =
        Assert.IsType<SymbolMappingNotFoundException>(font.GetCharInfo("unknownSymbol", TexStyle.Text).Error)

    [<Fact>]
    member _.``GetCharInfo(CharFont, TexStyle) returns a CharInfo for existing symbol``() =
        let char = CharFont('x', 1)
        Assert.NotNull <| font.GetCharInfo(char, TexStyle.Text)

    [<Fact>]
    member _.``GetCharInfo(CharFont, TexStyle) throws a TexCharacterMappingNotFoundException for unknown character``() =
        let char = CharFont('й', 1)
        Assert.IsType<TexCharacterMappingNotFoundException>(font.GetCharInfo(char, TexStyle.Text).Error)
