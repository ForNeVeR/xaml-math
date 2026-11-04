Matrices and Matrix-Like Constructs
===================================

XAML-Math supports multiple types of matrices and similar constructs (that allow to build the items into rows and columns).

All commands offer a similar syntax, but a bit different features. Rows inside of a matrix are separated by two commands:

- `\cr`
- `\\`

Cells inside of a row are separated by `&`.

For example, the following command creates a matrix with 2 rows and 3 columns:

```
\matrix{1 & 2 & 3 \\ 4 & 5 & 6}
```

The following matrix types are supported (each named after its delimiters, following LaTeX/amsmath):

- `\matrix`: a matrix without delimiters
- `\pmatrix`: a matrix within parentheses `( )`
- `\bmatrix`: a matrix within square brackets `[ ]`
- `\Bmatrix`: a matrix within curly braces `{ }`
- `\vmatrix`: a matrix within single vertical bars `| |`
- `\Vmatrix`: a matrix within double vertical bars `‖ ‖`

There's also a matrix-like construct:

- `\cases`: for piecewise functions etc

See also the documentation on [environments][].

[environments]: environments.md
