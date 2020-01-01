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

Parameters
----------

First section of the file contains the general parameters relevant to all of the fonts in the file. Here's an example of this section:

```xml
<Parameters num1="0.676508" num2="0.393732" num3="0.443731" denom1="0.685951" denom2="0.344841"
	sup1="0.412892" sup2="0.362892" sup3="0.288889" sub1="0.15" sub2="0.247217" supdrop="0.386108"
	subdrop="0.05" axisheight="0.25" defaultrulethickness="0.039999" bigopspacing1="0.111112"
	bigopspacing2="0.166667" bigopspacing3="0.2" bigopspacing4="0.6" bigopspacing5="0.1" />
```

All of these parameters can be directly extracted from the `*.tpl` files created by `tftopl`. Please note that not each TFM will contain all of the parameters; the result should be a combination of the parameters extracted from different font files.

For example, in Computer Modern of size 10, only one file includes the parameters

```
(FONTDIMEN
   (NUM1 R 0.676508)
   (NUM2 R 0.393732)
   (NUM3 R 0.443731)
)
```

and only one file (another one) includes the parameters such as `BIGOPSPACING1`.`

General settings
----------------

```xml
<GeneralSettings mufontid="3" spacefontid="1" scriptfactor="0.7" scriptscriptfactor="0.5" />
```

This section describes the following parameters:

- `mufontid`: an identifier of the font whose `block` size will be considered equal to TeX's `mu` (math unit)
- `spacefontid`: an identifier of the font whose `space` size will be considered as the default space size
- `scriptfactor`: a relative size of the elements inside of a script (superscript/subscript) element
- `scriptscriptfactor`: a relative size of the elements inside of a high-order script (i.e. a superscript/subscript inside of another script) element

Per-font settings
-----------------

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
