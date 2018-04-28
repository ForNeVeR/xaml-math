namespace WpfMath.Tests

open Xunit

open WpfMath
open WpfMath.Exceptions

type DefaultTexFontTests() =
    static do Utils.initializeFontResourceLoading()

    let font = DefaultTexFont(1.0)

    [<Fact>]
    member __.``GetCharInfo(char, string, TexStyle) returns a CharInfo for existing character``() =
        Assert.NotNull <| font.GetCharInfo('x', "text", TexStyle.Text)

    [<Fact>]
    member __.``GetCharInfo(char, string, TexStyle) throws a TextStyleMappingNotFoundException for unknown text style``() =
        Assert.Throws<TextStyleMappingNotFoundException>(
            fun () -> ignore <| font.GetCharInfo('x', "unknownStyle", TexStyle.Text))

    [<Fact>]
    member __.``GetCharInfo(string, TexStyle) returns a CharInfo for existing symbol``() =
        Assert.NotNull <| font.GetCharInfo("sqrt", TexStyle.Text)

    [<Fact>]
    member __.``GetCharInfo(string, TexStyle) throws a SymbolMappingNotFoundException for unknown symbol``() =
        Assert.Throws<SymbolMappingNotFoundException>(
            fun () -> ignore <| font.GetCharInfo("unknownSymbol", TexStyle.Text))

    [<Fact>]
    member __.``GetCharInfo(CharFont, TexStyle) returns a CharInfo for existing symbol``() =
        let char = CharFont('x', 1)
        Assert.NotNull <| font.GetCharInfo(char, TexStyle.Text)

    [<Fact>]
    member __.``GetCharInfo(CharFont, TexStyle) throws a TexCharacterMappingNotFoundException for unknown character``() =
        let char = CharFont('й', 1)
        Assert.Throws<TexCharacterMappingNotFoundException>(
            fun () -> ignore <| font.GetCharInfo(char, TexStyle.Text))
