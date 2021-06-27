module WpfMath.Tests.TexRendererTests

open WpfMath
open Xunit

[<Fact>]
let ``TexRenderer.RenderToBitmap should create an image of proper size``() =
    let parser = TexFormulaParser()
    let formula = parser.Parse "2+2=2"
    let renderer = formula.GetRenderer(TexStyle.Display, 20.0, "Arial")
    let bitmap = renderer.RenderToBitmap(0.0, 0.0)
    Assert.Equal(82, bitmap.PixelWidth)
    Assert.Equal(17, bitmap.PixelHeight)

[<Fact>]
let ``TexRenderer.RenderToBitmap should create an image of proper size with offset``() =
    let parser = TexFormulaParser()
    let formula = parser.Parse "2+2=2"
    let renderer = formula.GetRenderer(TexStyle.Display, 20.0, "Arial")
    let margin = 50
    let bitmap = renderer.RenderToBitmap(float(margin), float(margin))

    let formulaWidth = 82
    let formulaHeight = 16
    Assert.Equal(formulaWidth + (margin * 2), bitmap.PixelWidth)
    Assert.Equal(formulaHeight + (margin * 2), bitmap.PixelHeight)

[<Fact>]
let ``TexRenderer.RenderToBitmap should work with different DPI``() =
    let parser = TexFormulaParser()
    let formula = parser.Parse "2+2=2"
    let renderer = formula.GetRenderer(TexStyle.Display, 20.0, "Arial")
    let bitmap = renderer.RenderToBitmap(0.0, 0.0, 192.0)
    Assert.Equal(163, bitmap.PixelWidth)
    Assert.Equal(34, bitmap.PixelHeight)
    Assert.Equal(192.0, bitmap.DpiX)
    Assert.Equal(192.0, bitmap.DpiY)
