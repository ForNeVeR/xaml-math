How to prepare XML from TTF
===========================

1. To gather font metrics:
   ```console
   $ ttf2tfm filename.ttf -v filename.vpl
   $ tftopl filename.tfm > filename.tpl
   ```
   That will generate `filename.tfm` (binary) and `filename.vpl`, `filename.tpl`
   (text).

   `ttf2tfm` and `tftopl` are parts of the standard TeX distribution (at least
   TeX Live).
2. May be helpful:
   ```console
   $ ttx filename.ttf
   ```

   That will generate `filename.ttx` (XML).

   [ttx][]

[ttx]: https://github.com/fonttools/fonttools
