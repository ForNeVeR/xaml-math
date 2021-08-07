WPF-Math [![NuGet][badge-nuget]][nuget]
========

*WPF-Math* is a .NET library for rendering mathematical formulae using the LaTeX typesetting style, for the WPF framework.

It supports the following .NET runtimes:
- .NET Framework 4.5.2 or later
- .NET Core 3.1 or later
- .NET 5.0 or later

Getting Started
---------------

The simplest way of using *WPF-Math* is to render a static formula in a XAML file as follows.

```xml
<Window ... xmlns:controls="clr-namespace:WpfMath.Controls;assembly=WpfMath">
    <controls:FormulaControl Formula="\left(x^2 + 2 \cdot x + 2\right) = 0" />
</Window>
```

For a more detailed sample, check out the [example project][example]. It shows the usage of data binding and some advanced concepts.

![Screenshot of example project](docs/example-screenshot.png)

### Using a rendering API

The following example demonstrates usage of `TexFormula` API to render the image into a PNG file using the `RenderToPng` extension method:

```csharp
using System.IO;
using WpfMath;

namespace ConsoleApplication2
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            const string latex = @"\frac{2+2}{2}";
            const string fileName = @"T:\Temp\formula.png";

            var parser = new TexFormulaParser();
            var formula = parser.Parse(latex);
            var pngBytes = formula.RenderToPng(20.0, 0.0, 0.0, "Arial");
            File.WriteAllBytes(fileName, pngBytes);
        }
    }
}
```

If you need any additional control over the image format, consider using the `GetRenderer` API:

```csharp
using System;
using System.IO;
using System.Windows.Media.Imaging;
using WpfMath;

namespace ConsoleApplication2
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            const string latex = @"\frac{2+2}{2}";
            const string fileName = @"T:\Temp\formula.png";

            var parser = new TexFormulaParser();
            var formula = parser.Parse(latex);
            var renderer = formula.GetRenderer(TexStyle.Display, 20.0, "Arial");
            var bitmapSource = renderer.RenderToBitmap(0.0, 0.0);
            Console.WriteLine($"Image width: {bitmapSource.Width}");
            Console.WriteLine($"Image height: {bitmapSource.Height}");

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            using (var target = new FileStream(fileName, FileMode.Create))
            {
                encoder.Save(target);
                Console.WriteLine($"File saved to {fileName}");
            }
        }
    }
}
```

You may also pass your own `IElementRenderer` implementation to `TexFormula.RenderFormulaTo` method if you need support for any alternate rendering engines.

Documentation
-------------

- [Changelog][docs.changelog]

- [Color support in WPF-Math][docs-colors]
- [Matrices and Matrix-Like Constructs][docs-matrices]
- [How to improve blurred formulas][docs-blurred-text-issue]

- [How to prepare `DefaultTexFont.xml` from the font file][docs-prepare-font]

- [Licensing history][docs-licensing-history]

- [Maintainership][docs.maintainership]

Build and Maintenance Instructions
----------------------------------

Build the project using [.NET SDK 5.0][dotnet-sdk]. WPF-Math requires C# 8 and F# 4.7 support. Here's the build and test script:

```console
$ dotnet build --configuration Release
$ dotnet test
```

To approve the test results if they differ from the existing ones, execute the `scripts/approve-all.ps1` script using PowerShell or PowerShell Core.

To publish the package, execute the following command:

```console
$ dotnet pack --configuration Release
```

History
-------

The library was originally ported from the [JMathTex project][jmathtex], copyright 2004-2007 Universiteit Gent. The port was originally named *WPF-TeX* and was written and maintained by [Alex Regueiro][alex-regueiro]. It was later available as [*WPF-Math* on Launchpad][launchpad], but was unmaintained from 2011 until it was revived in [its current form][github].

License Notes
-------------

The project code and all the resources are distributed under the terms of [MIT license][license].

The [fonts][] `cmex10.ttf`, `cmmi10.ttf`, `cmr10.ttf`, and `cmsy10.ttf` and `cmtt10.ttf` are under the [Knuth License][knuth-license].

WPF-Math started as a direct port of [JMathTeX][jmathtex] project written in Java, reusing both code and resources. JMathTeX is distributed under the terms of GNU GPL v2 license. WPF-Math, being a derived work, has a permission from JMathTeX authors to be redistributed under the MIT license. See the [Licensing history][docs-licensing-history] for the details.

We're very grateful to JMathTeX authors for their work and allowing to redistribute the derived library. JMathTeX is written by:
- Kris Coolsaet
- Nico Van Cleemput
- Kurt Vermeulen

[docs-blurred-text-issue]: docs/blurred-text-issue.md
[docs-colors]: docs/colors.md
[docs-licensing-history]: docs/licensing-history.md
[docs-matrices]: docs/matrices.md
[docs-prepare-font]: docs/prepare-font.md
[docs.changelog]: ./CHANGELOG.md
[docs.maintainership]: ./MAINTAINERSHIP.md
[example]: src/WpfMath.Example/
[fonts]: src/WpfMath/Fonts/
[license]: LICENSE.md

[alex-regueiro]: https://github.com/alexreg
[dotnet-sdk]: https://dotnet.microsoft.com/download
[github]: https://github.com/ForNeVeR/wpf-math
[jmathtex]: http://jmathtex.sourceforge.net/
[knuth-license]: http://ctan.org/license/knuth
[launchpad]: https://launchpad.net/wpf-math
[msbuild]: https://github.com/Microsoft/msbuild
[nuget]: https://www.nuget.org/packages/WpfMath/

[badge-nuget]: https://img.shields.io/nuget/v/WpfMath.svg
