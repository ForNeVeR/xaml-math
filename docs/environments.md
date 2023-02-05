Environments (`\begin` and `\end`)
==================================
XAML-Math supports _environments_: ability to introduce certain context for the nested markup.

`\begin{environment-name}` starts an environment named `environment-name`, `\end{environment-name}` ends it. Every `\begin` should be followed by a corresponding `\end`. Environments could be nested.

List of currently supported environment names:

- `pmatrix`: works the same as the [corresponding matrix command][docs.matrices].

Examples:
```tex
\begin{pmatrix} a & b & c \\ d & e & f \end{pmatrix}
```
This works the same as:
```tex
\pmatrix{a & b & c \\ d & e & f}
```

[docs.matrices]: matrices.md
