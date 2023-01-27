module WpfMath.Tests.RenderingTests

open System.Windows.Media

open Xunit

open WpfMath.Parsers
open WpfMath.Rendering

let private parser = WpfTeXFormulaParser.Instance

[<Fact>]
let ``WpfTeXFormulaExtensions::RenderToBitmap should create an image of proper size``(): unit =
    let formula = parser.Parse "2+2=2"
    let environment = WpfTeXEnvironment.Create()
    let bitmap = formula.RenderToBitmap environment
    Assert.Equal(82, bitmap.PixelWidth)
    Assert.Equal(17, bitmap.PixelHeight)

[<Fact>]
let ``WpfTeXFormulaExtensions::RenderToBitmap should create an image of proper size with offset``(): unit =
    let formula = parser.Parse "2+2=2"
    let environment = WpfTeXEnvironment.Create()
    let margin = 50
    let bitmap = formula.RenderToBitmap(environment, x = float margin, y = float margin)

    let formulaWidth = 82
    let formulaHeight = 16
    Assert.Equal(formulaWidth + (margin * 2), bitmap.PixelWidth)
    Assert.Equal(formulaHeight + (margin * 2), bitmap.PixelHeight)

[<Fact>]
let ``WpfTeXFormulaExtensions::RenderToBitmap should work with different DPI``(): unit =
    let formula = parser.Parse "2+2=2"
    let environment = WpfTeXEnvironment.Create()
    let bitmap = formula.RenderToBitmap(environment, dpi = 192.0)
    Assert.Equal(163, bitmap.PixelWidth)
    Assert.Equal(34, bitmap.PixelHeight)
    Assert.Equal(192.0, bitmap.DpiX)
    Assert.Equal(192.0, bitmap.DpiY)

[<Fact>]
let ``WpfTeXFormulaExtensions::RenderToGeometry should work``() =
    let formula = parser.Parse "2+2=2"
    let environment = WpfTeXEnvironment.Create()
    let geometry = formula.RenderToGeometry(environment) :?> GeometryGroup
    Assert.Equal(5, geometry.Children.Count)
