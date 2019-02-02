WPF-Math [![Build status][badge-appveyor]][appveyor] [![NuGet][badge-nuget]][nuget]
========

*WPF-Math* is a .NET library for rendering mathematical formulae using the LaTeX typsetting style, for the WPF framework.

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

- [How to prepare `DefaultTexFont.xml` from the font file][docs-prepare-font]

- [Licensing history][docs-licensing-history]

Build Instructions
------------------

Build the project using [MSBuild][msbuild] or any compatible environment (e.g. Visual Studio 2017 or Rider). WPF-Math requires C# 7.2 support. Build script:

```console
$ nuget restore
$ msbuild /p:Configuration=Release
```

To run the unit tests, use any xunit-compatible runner (e.g. Visual Studio 2017 or Rider).

To publish the package, execute the following command with [PowerShell][pwsh]:

```console
$ pwsh scripts/nuget-pack.ps1
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

[docs-prepare-font]: docs/prepare-font.md
[docs-licensing-history]: docs/licensing-history.md
[example]: WpfMath.Example/
[fonts]: src/WpfMath/Fonts/
[license]: LICENSE.md

[alex-regueiro]: https://github.com/alexreg
[appveyor]: https://ci.appveyor.com/project/ForNeVeR/wpf-math/branch/master
[github]: https://github.com/ForNeVeR/wpf-math
[jmathtex]: http://jmathtex.sourceforge.net/
[knuth-license]: http://ctan.org/license/knuth
[launchpad]: https://launchpad.net/wpf-math
[msbuild]: https://github.com/Microsoft/msbuild
[nuget]: https://www.nuget.org/packages/WpfMath/
[pwsh]: https://github.com/PowerShell/PowerShell

[badge-appveyor]: https://ci.appveyor.com/api/projects/status/b26m3rpfcgb91gdg/branch/master?svg=true
[badge-nuget]: https://img.shields.io/nuget/v/WpfMath.svg
