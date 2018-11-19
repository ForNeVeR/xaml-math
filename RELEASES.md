Release Notes
=============

## 0.6.0 (2018-11-19)

Fixed issues:

- [#60: Command arguments passed without braces][issue-60]
- [#168: Poor exported PNG quality and text cutting][issue-168]

New features:

- [#91: \overline command][issue-91]
- [#145: Implemented the underline command][pull-145]

Refactorings:

- [#152: Move converters to namespace][pull-152]

## 0.5 (2018-05-08)

Fixed issues:

- [#101: Fixed crash on empty square root][pull-101]
- [#103: Fixed a crash if inserting a whitespace after a _ or ^ symbol][pull-103]
- [#102: Fixed a crash when rendering unsupported characters like "Å"][pull-102]
- [#104: Error when rendering `\text{æ,}`][issue-104]
- [#119: Big operator rendering problem inside `\frac`][issue-119]
- [#129: Summation of two Cyrillic symbols][issue-129]
- [#133: Fix issues with exceptions][pull-133]

New features:

- [#92: Extract Renderer code to a single class/interface][issue-92]
- [#115: Give a range of source string to Box][issue-115]
- [#123: Add formula highlighting][pull-123]

New documentation:

- [#108: Dealing with XML][pull-108]

## 0.4 (2017-09-11)

Fixed issues:

- [#80: force streamwriter to flush buffers][pull-80]
- [#88: Bar alignment][issue-88]

New features:

- [#79: SVG: Added support for curves and removed flattening of the geometry][pull-79]
- [#82: Support UTF8][issue-82]
- [#84: Add \text{} command][issue-84]

## 0.3.1 (2017-03-18)

Bug fixes, improved render quality.

Changes:

- [#53: Fix crash while parsing empty groups][pull-53]
- [#54: Remove delimiter type constraint][pull-54]
- [#58: Support for empty delimiters][pull-58] _(that means [#14: Extensible delimiters (\left and \right)][issue-14] is completely fixed)_
- [#71: Use guidelines for WPF render][pull-71] _(partial fix for [#50: Improve blurred output][issue-50], should improve image quality for some cases)_
- [Adjust metrics for radical symbol][commit-14c303d]
- [#74: Add support for scripts to delimiters][pull-74] (fix for [#62: Delimiters should support scripts][issue-62])

## 0.3.0 (2017-02-24)

Fixed issues:

- [#14: Extensible delimiters (\left and \right)][issue-14] _(not completely fixed, but in a usable state already)_
- #15: Add formula control to the library
- #38: Cannot change text style
- #32: Add mathematical functions
- #24: Integral sign displays incorrectly

## 0.2.0 (2017-02-16)

Merged all the patches from the Launchpad repository (see #3 for a full list).
The most interesting ones are:

- Fix for culture-dependent number parsing in `XmlUtils`
- Addition of SVG rendering functionality

## 0.1.0 (2017-02-11)

This was the initial published version. It consisted entirely of the original code from the Launchpad projectsite.

[commit-14c303d]: https://github.com/ForNeVeR/wpf-math/commit/14c303d30eba735af4faa5e72e149c60add00293
[issue-14]: https://github.com/ForNeVeR/wpf-math/issues/14
[issue-50]: https://github.com/ForNeVeR/wpf-math/issues/50
[issue-60]: https://github.com/ForNeVeR/wpf-math/issues/60
[issue-62]: https://github.com/ForNeVeR/wpf-math/issues/62
[issue-82]: https://github.com/ForNeVeR/wpf-math/issues/82
[issue-84]: https://github.com/ForNeVeR/wpf-math/issues/84
[issue-88]: https://github.com/ForNeVeR/wpf-math/issues/84
[issue-91]: https://github.com/ForNeVeR/wpf-math/issues/91
[issue-92]: https://github.com/ForNeVeR/wpf-math/issues/92
[issue-104]: https://github.com/ForNeVeR/wpf-math/issues/104
[issue-115]: https://github.com/ForNeVeR/wpf-math/issues/115
[issue-119]: https://github.com/ForNeVeR/wpf-math/issues/119
[issue-129]: https://github.com/ForNeVeR/wpf-math/issues/129
[issue-168]: https://github.com/ForNeVeR/wpf-math/issues/168
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
