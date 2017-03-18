Release Notes
=============

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
[issue-62]: https://github.com/ForNeVeR/wpf-math/issues/62
[pull-53]: https://github.com/ForNeVeR/wpf-math/pull/53
[pull-54]: https://github.com/ForNeVeR/wpf-math/pull/54
[pull-58]: https://github.com/ForNeVeR/wpf-math/pull/58
[pull-71]: https://github.com/ForNeVeR/wpf-math/pull/71
[pull-74]: https://github.com/ForNeVeR/wpf-math/pull/74
