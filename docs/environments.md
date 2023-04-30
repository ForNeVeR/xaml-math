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

- `align`: used for equation alignment.

  For example, this will make the left and right parts of equations aligned:
  ```tex
  \begin{align} x+1 &= y + 1 \\ x &= y-1 \end{align}
  ```

  And this will allow to put the equations into several columns:
  ```tex
  \begin{align} x+1 &= y + 1 & a &= b + 1 \\ x &= y-1 & b + a &= c \end{align}
  ```

[docs.matrices]: matrices.md
