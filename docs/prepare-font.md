How to prepare `DefaultTexFont.xml` from the font file
======================================================

This document describes the correspondence between the font files we use and
data in `DefaultTexFont.xml`. The concrete approach (i.e. script files to
regenerate XML data) hasn't been reproduced yet, although we were able to get
nearly all the required information.

Data in our XML files is often inaccurate and/or seemingly generated from other
TTF files than the files in the repository. Be prepared for that. When in doubt,
refer to [the original TFM files][tfm] (they seem to correspond to our data
better than the TFM files generated from our TTF files).

## `Tool.TTFMetrics`

There's a WPF-based `Tool.TTFMetrics` application designed for gathering the necessary font information.

Start it, choose a `.ttf` file, and it will generate the data to paste into the `<Font>` section of the `DefaultTexFont.xml` file.

## Manual Approach

There are two helpful toolsets to work with our TTF files:

1. To gather font metrics:
   ```console
   $ ttf2tfm filename.ttf -v filename.vpl
   $ tftopl filename.tfm > filename.tpl
   ```
   That will generate `filename.tfm` (binary) and `filename.vpl`, `filename.tpl`
   (text).

   `ttf2tfm` and `tftopl` are parts of the standard TeX distribution (at least
   TeX Live).
2. It may be helpful to use the [ttx][] utility to get some additional
   information about the font (e.g. mapping from characters to codes in the font
   file):
   ```console
   $ ttx filename.ttf
   ```

   That will generate `filename.ttx` (XML).

For example, there's the following in `DefaultTexFont.xml`:

```xml
<Font name="cmmi10.ttf" id="0" space="0.0" xHeight="0.430555" skewChar="196" quad="1.000003">
    <Char code="65" width="0.750002" height="0.683332" >
        <Kern code="196" val="0.138893"/>
    </Char>
</Font>
```

To get these values from the `cmmi10.tpl` file, search for the following:

```
(FONTDIMEN
   (SLANT R 0.25)
   (SPACE R 0.0)
   (STRETCH R 0.0)
   (SHRINK R 0.0)
   (XHEIGHT R 0.430555)
   (QUAD R 1.000003)
   )

…

(CHARACTER C A
   (CHARWD R 0.750002)
   (CHARHT R 0.683332)
   (COMMENT
      (KRN O 177 R 0.138893)
      )
   )
```

Here, `CHARWD` corresponds to `width`, `CHARHT` corresponds to `height`,
`CHARDP` corresponds to `depth`, and `CHARIC` corresponds to `italic`. Any of
them could be omitted.

The kerning section can also be reconstructed from the `*.tpl` file.

How can we know that `O 177` is the same as `code="196"`? To do that, first look
into `cmmi10.vpl` file: there's the following entry:

```
(CHARACTER O 177 (comment dieresis)
   (CHARWD R 583)
   (CHARHT R 705)
   (CHARIC R 118)
   (MAP
      (SETCHAR O 151)
      )
   )
```

That means that `O 177` is named `dieresis`. Then, open [Adobe Glyph
List][glyphlist] and search for the `dieresis` name:

```
dieresis;00A8
```

It means that `O 177` should be character `0xa8` or `168`, not `196`. The reason
for that inconsistency is currently unknown.

[glyphlist]: https://github.com/adobe-type-tools/agl-aglfn/blob/5de337bfa018e480bf15b77973e27ccdbada8e56/glyphlist.txt
[tfm]: https://ctan.org/texarchive/fonts/cm/tfm
[ttx]: https://github.com/fonttools/fonttools
