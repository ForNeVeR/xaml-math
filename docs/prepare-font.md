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

For example, there's the following in `DefaultTexFont.xml`:

```xml
<Font name="cmmi10.ttf" id="0" space="0.0" xHeight="0.430555" skewChar="196" quad="1.000003">
    <Char code="65" width="0.750002" height="0.683332" >
        <Kern code="196" val="0.138893"/>
    </Char>
</Font>
```

1. Grab the original `.mf` and `.tfm` files from which the font was constructed (`.tfm` are supposed to be created from `.mf`, but they are already available for Computer Modern, so there's no reason to make it any harder).
2. Remember the `skewchar` value. For example, for `cmmi10.mf` it seems to be `skewchar=oct"177"` from `mathit.mf`. Remember the oct value of `177`.
3. Run `tftopl cmmi10.tfm > cmmi10.tpl` to get a `.tpl` file
4. From now on, it's straightforward.
   - Prepare the `<Font>` element:
     - `name` is the name of the `.ttf` file
     - `id` is the font id (just a sequential number)
     - `space`: TODO!
     - `xHeight` is `XHEIGHT` from the `FONTDIMEN` section of the `.tpl` file
     - `skewChar` is the oct value of the `skewchar` from the `.mf` file, but converted into decimal
     - `quad` is `QUAD` from the `FONTDIMEN` section of the `.tpl` file
   - Prepare the `<Char>` elements sequentially:
     - First two atoms after `(CHARACTER` are the character identifier, either written on octal `O <octal-number>` or as a real letter `C <letter>` (you are supposed to map it to the letter code in ASCII then). This identifier doesn't mean how the font character is actually used, and is just for identifying purposes (the characters may refer to each other in the kerning section).
     - `CHARWD` is `width`, `CHARHT` is `height`, `CHARDP` is `depth`, and `CHARIC` is `italic`. The latter two may be omitted.
     - `char` attribute should come from the font mapping // TODO: how to get it?
     - The kerning section is filled by parsing the `COMMENT`: for example, `(KRN O 177 R 0.083336)` means we need the `char` from the character id `O 177`, and `val="0.083336"`.

TODO: How to reconstruct the `SymbolMapping` items having the original TeX sources?

TODO: How to recreate `<NextLarger>` element from the original TeX sources?

[tfm]: https://ctan.org/texarchive/fonts/cm/tfm
