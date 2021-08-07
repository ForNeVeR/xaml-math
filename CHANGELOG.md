# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning v2.0.0](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Added
- [#262: Add \mod operator from amsmath][pull-262]

## [0.10.0] - 2021-07-06
### Changed
- (**Breaking change!**) Removed support for .NET Core 3.0. .NET Core 3.1 or later is supported from now (.NET Framework 4.5.2 is still supported; .NET 5.0 or later is supported, too).

### Added
- [#277: Enable nullable reference types][pull-277]

### Fixed
- [#99: `Foreground` property not working on `FormulaControl`][issue-99]
- [#283: Fix typo in `SystemTextFontNameProperty`][pull-283]
- [#244: `\limsup` throws exception][issue-244]
- [#254: Fix for scripts with curly braces after a command with curly braces][pull-254] (e.g. `\hat{x}_{y}`)
- [#261: Crash on empty `\sqrt{}`][pull-261]
- [#275: `OverUnderBox` constructor may dereference scriptBox parameter if it's `null`][issue-275]

## [0.9.0] - 2020-07-31
### Added
- [#59: Extended delimiter support][issue-59]: e.g. `\left\\`
- [#149: Newline command support][issue-149]: try using `\\` outside of a matrix
- [#252: Support for \\{ and \\} commands][pull-252]

### Fixed
- [#139: Exception thrown by \\,][issue-139]
- [#151: Wrong sources detected for complex predefined formulae][issue-151]
- [#225: \text doesn't work with indices if there's only one Cyrillic letter][issue-225]
- [#248: Wrong exception gets thrown for \text{∅}][issue-248]
- [#253: Added equal padding to all sides when saving to bitmap][pull-253]
- [#257: IndexOutOfRangeException throws when using \color][issue-257]

## [0.8.0] - 2020-01-03
### Added
- [#165: Extended color support for \color and \colorbox][issue-165], see [the documentation][docs.colors]
- [#174: \binom command][issue-174]
- [#209: WPF support on .NET Core 3.0][issue-209]
- [#226: Add TFM into nuspec][issue-226]

### Fixed
- [#128: \colorbox renders radical index invisible][issue-128]
- [#158: \color{red} crashes parser][issue-158]
- [#203: `\colorbox{red}{}` crashes the parser][issue-203]
- [#212: `\mathrm{}` shouldn't throw exn][pull-212]

## [0.7.0] - 2019-08-24
### Changed
- (**Breaking change!**) [#198: Migrate to .NET 4.5.2][issue-198]

### Fixed
- [#141: _ doesn't render inside \text][issue-141]
- [#171: Make \lbrack and \lbrack commands for parentheses][issue-171]
- [#172: NullReferenceException on some types of markup][pull-172]

### Added
- [#100: Support for matrix commands][issue-100]

## [0.6.0] - 2018-11-19
### Fixed
- [#60: Command arguments passed without braces][issue-60]
- [#168: Poor exported PNG quality and text cutting][issue-168]

### Added
- [#91: \overline command][issue-91]
- [#145: Implemented the underline command][pull-145]

### Changed

- (Refactoring) [#152: Move converters to namespace][pull-152]

## [0.5.0] - 2018-05-08
### Fixed
- [#101: Fixed crash on empty square root][pull-101]
- [#103: Fixed a crash if inserting a whitespace after a _ or ^ symbol][pull-103]
- [#102: Fixed a crash when rendering unsupported characters like "Å"][pull-102]
- [#104: Error when rendering `\text{æ,}`][issue-104]
- [#119: Big operator rendering problem inside `\frac`][issue-119]
- [#129: Summation of two Cyrillic symbols][issue-129]
- [#133: Fix issues with exceptions][pull-133]

### Added
- [#92: Extract Renderer code to a single class/interface][issue-92]
- [#115: Give a range of source string to Box][issue-115]
- [#123: Add formula highlighting][pull-123]
- (Documentation) [#108: Dealing with XML][pull-108]

## [0.4.0] - 2017-09-11
### Fixed
- [#80: force streamwriter to flush buffers][pull-80]
- [#88: Bar alignment][issue-88]

### Added
- [#79: SVG: Added support for curves and removed flattening of the geometry][pull-79]
- [#82: Support UTF8][issue-82]
- [#84: Add \text{} command][issue-84]

## [0.3.1] - 2017-03-18
### Changed
- Bug fixes, improved render quality
- [#53: Fix crash while parsing empty groups][pull-53]
- [#54: Remove delimiter type constraint][pull-54]
- [#58: Support for empty delimiters][pull-58] _(that means [#14: Extensible delimiters (\left and \right)][issue-14] is completely fixed)_
- [#71: Use guidelines for WPF render][pull-71] _(partial fix for [#50: Improve blurred output][issue-50], should improve image quality for some cases)_
- [Adjust metrics for radical symbol][commit-14c303d]
- [#74: Add support for scripts to delimiters][pull-74] (fix for [#62: Delimiters should support scripts][issue-62])

## [0.3.0] - 2017-02-24
### Fixed
- [#14: Extensible delimiters (\left and \right)][issue-14] _(not completely fixed, but in a usable state already)_
- [#15: Add formula control to the library][issue-15]
- [#38: Cannot change text style][issue-38]
- [#32: Add mathematical functions][issue-32]
- [#24: Integral sign displays incorrectly][issue-24]

## [0.2.0] - 2017-02-16
Merged all the patches from the Launchpad repository (see #3 for a full list). The most interesting ones are:

- Fix for culture-dependent number parsing in `XmlUtils`
- Addition of SVG rendering functionality

## [0.1.0] - 2017-02-11
This was the initially published version. It consisted entirely of the original code from the Launchpad project site.

[docs.colors]: docs/colors.md

[commit-14c303d]: https://github.com/ForNeVeR/wpf-math/commit/14c303d30eba735af4faa5e72e149c60add00293
[issue-14]: https://github.com/ForNeVeR/wpf-math/issues/14
[issue-15]: https://github.com/ForNeVeR/wpf-math/issues/15
[issue-24]: https://github.com/ForNeVeR/wpf-math/issues/24
[issue-32]: https://github.com/ForNeVeR/wpf-math/issues/32
[issue-38]: https://github.com/ForNeVeR/wpf-math/issues/38
[issue-50]: https://github.com/ForNeVeR/wpf-math/issues/50
[issue-59]: https://github.com/ForNeVeR/wpf-math/issues/59
[issue-60]: https://github.com/ForNeVeR/wpf-math/issues/60
[issue-62]: https://github.com/ForNeVeR/wpf-math/issues/62
[issue-82]: https://github.com/ForNeVeR/wpf-math/issues/82
[issue-84]: https://github.com/ForNeVeR/wpf-math/issues/84
[issue-88]: https://github.com/ForNeVeR/wpf-math/issues/84
[issue-91]: https://github.com/ForNeVeR/wpf-math/issues/91
[issue-92]: https://github.com/ForNeVeR/wpf-math/issues/92
[issue-99]: https://github.com/ForNeVeR/wpf-math/issues/99
[issue-100]: https://github.com/ForNeVeR/wpf-math/issues/100
[issue-104]: https://github.com/ForNeVeR/wpf-math/issues/104
[issue-115]: https://github.com/ForNeVeR/wpf-math/issues/115
[issue-119]: https://github.com/ForNeVeR/wpf-math/issues/119
[issue-128]: https://github.com/ForNeVeR/wpf-math/issues/128
[issue-129]: https://github.com/ForNeVeR/wpf-math/issues/129
[issue-139]: https://github.com/ForNeVeR/wpf-math/issues/139
[issue-141]: https://github.com/ForNeVeR/wpf-math/issues/141
[issue-149]: https://github.com/ForNeVeR/wpf-math/issues/149
[issue-151]: https://github.com/ForNeVeR/wpf-math/issues/151
[issue-158]: https://github.com/ForNeVeR/wpf-math/issues/158
[issue-165]: https://github.com/ForNeVeR/wpf-math/issues/165
[issue-168]: https://github.com/ForNeVeR/wpf-math/issues/168
[issue-171]: https://github.com/ForNeVeR/wpf-math/issues/171
[issue-174]: https://github.com/ForNeVeR/wpf-math/issues/174
[issue-198]: https://github.com/ForNeVeR/wpf-math/issues/198
[issue-203]: https://github.com/ForNeVeR/wpf-math/issues/203
[issue-209]: https://github.com/ForNeVeR/wpf-math/issues/209
[issue-225]: https://github.com/ForNeVeR/wpf-math/issues/225
[issue-226]: https://github.com/ForNeVeR/wpf-math/issues/226
[issue-244]: https://github.com/ForNeVeR/wpf-math/issues/244
[issue-248]: https://github.com/ForNeVeR/wpf-math/issues/248
[issue-257]: https://github.com/ForNeVeR/wpf-math/issues/257
[issue-275]: https://github.com/ForNeVeR/wpf-math/issues/275
[pull-53]: https://github.com/ForNeVeR/wpf-math/pull/53
[pull-54]: https://github.com/ForNeVeR/wpf-math/pull/54
[pull-58]: https://github.com/ForNeVeR/wpf-math/pull/58
[pull-71]: https://github.com/ForNeVeR/wpf-math/pull/71
[pull-74]: https://github.com/ForNeVeR/wpf-math/pull/74
[pull-79]: https://github.com/ForNeVeR/wpf-math/pull/79
[pull-80]: https://github.com/ForNeVeR/wpf-math/pull/80
[pull-101]: https://github.com/ForNeVeR/wpf-math/pull/101
[pull-102]: https://github.com/ForNeVeR/wpf-math/pull/102
[pull-103]: https://github.com/ForNeVeR/wpf-math/pull/103
[pull-108]: https://github.com/ForNeVeR/wpf-math/pull/108
[pull-123]: https://github.com/ForNeVeR/wpf-math/pull/123
[pull-133]: https://github.com/ForNeVeR/wpf-math/pull/133
[pull-145]: https://github.com/ForNeVeR/wpf-math/pull/145
[pull-152]: https://github.com/ForNeVeR/wpf-math/pull/152
[pull-172]: https://github.com/ForNeVeR/wpf-math/pull/172
[pull-212]: https://github.com/ForNeVeR/wpf-math/pull/212
[pull-252]: https://github.com/ForNeVeR/wpf-math/pull/252
[pull-253]: https://github.com/ForNeVeR/wpf-math/pull/253
[pull-254]: https://github.com/ForNeVeR/wpf-math/pull/254
[pull-261]: https://github.com/ForNeVeR/wpf-math/pull/261
[pull-262]: https://github.com/ForNeVeR/wpf-math/pull/262
[pull-277]: https://github.com/ForNeVeR/wpf-math/pull/277
[pull-283]: https://github.com/ForNeVeR/wpf-math/pull/283

[0.1.0]: https://github.com/ForNeVeR/wpf-math/releases/tag/0.1.0
[0.2.0]: https://github.com/ForNeVeR/wpf-math/compare/0.1.0...0.2.0
[0.3.0]: https://github.com/ForNeVeR/wpf-math/compare/0.2.0...0.3.0
[0.3.1]: https://github.com/ForNeVeR/wpf-math/compare/0.3.0...0.3.1
[0.4.0]: https://github.com/ForNeVeR/wpf-math/compare/0.3.1...0.4.0
[0.5.0]: https://github.com/ForNeVeR/wpf-math/compare/0.4.0...0.5.0
[0.6.0]: https://github.com/ForNeVeR/wpf-math/compare/0.5.0...0.6.0
[0.7.0]: https://github.com/ForNeVeR/wpf-math/compare/0.6.0...0.7.0
[0.8.0]: https://github.com/ForNeVeR/wpf-math/compare/0.7.0...0.8.0
[0.9.0]: https://github.com/ForNeVeR/wpf-math/compare/0.8.0...0.9.0
[0.10.0]: https://github.com/ForNeVeR/wpf-math/compare/0.9.0...v0.10.0
[Unreleased]: https://github.com/ForNeVeR/wpf-math/compare/v0.10.0...HEAD
