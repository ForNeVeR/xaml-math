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

The project code is licensed under the terms of [MIT license][license]. The original resources from [JMathTeX][jmathtex] (`DefaultTexFont.xml`, `GlueSettings.xml`, `PredefinedTexFormulas.xml`, `TexFormulaSettings.xml`, `TexSymbols.xml`) are taken from the [GPLv2-distributed][gpl] [JMathTeX][jmathtex], but JMathTeX authors have granted permission to redistribute these resourses under the MIT license. See the [wiki][wiki-license-info] for details.

The [fonts][] `cmex10.ttf`, `cmmi10.ttf`, `cmr10.ttf`, and `cmsy10.ttf` and `cmtt10.ttf` are under the [Knuth License][knuth-license].

[docs-prepare-font]: docs/prepare-font.md
[example]: WpfMath.Example/
[fonts]: src/WpfMath/Fonts/
[gpl]: docs/JMathTeX-license.txt
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
[wiki-license-info]: https://github.com/ForNeVeR/wpf-math/wiki/Additional-license-info

[badge-appveyor]: https://ci.appveyor.com/api/projects/status/b26m3rpfcgb91gdg/branch/master?svg=true
[badge-nuget]: https://img.shields.io/nuget/v/WpfMath.svg
