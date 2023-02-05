Color support in XAML-Math
==========================

XAML-Math supports standard LaTeX commands: `\color` for foreground color, and `\colorbox` for background color.

The commands also support additional syntax for color definitions. Standard LaTeX only allows to use predefined colors or colors defined in the same document. XAML-Math allows immediate color definition in the command arguments instead.

The full color command syntax is:

```
\command [mode] {color} {text}
```

Where:
- `command` is either `color` or `colorbox`
- `mode` is an optional argument that could be one of
    - `gray`
    - `RGB`, `rgb`, `ARGB`, `argb`, `RGBA`, `rgba`
    - `cmyk`
    - `HTML`
- `color` is either a predefined color name if the `mode` argument wasn't
  provided, or a color definition according to the `mode` argument

Some of the modes accepts multiple numbers separated by comma `,`; whitespace is
allowed around the comma, before first and after the last element in the list.

## Default mode (when no `mode` was provided)

In the default mode, `color` should be a predefined color name. Predefined colors are stored in the `PredefinedColors.xml` file from the XAML-Math resources. Additionally, this mode accepts an opacity index in range from 0.0 to 1.0. Examples:

```
\color{red}{without opacity}
\color{red, 0.5}{with opacity}
```

## `gray` mode

`gray` mode accepts a grayscale tone in range from 0.0 to 1.0. Additionally,
this mode accepts an opacity index in range from 0.0 to 1.0. Examples:

```
\color[gray]{0.5}{without opacity}
\color[gray]{0.5,0.5}{with opacity}
```

## RGB-based modes

There're six RGB-based modes: capital ones (`RGB`, `RGBA`, `ARGB`) accept three
(or four with opacity) comma-separated values in range from 0 to 255, lowercase
ones (`rgb`, `rgba`, `argb`) accept three (or four) comma-separated values in
range from 0.0 to 1.0. `ARGB` accepts opacity index first, `RGBA` accepts it
last. Examples:

```
\colorbox[rgb]{0,0,0}{text}
\colorbox[rgba]{0,0,0, 0.5}{opacity = 0.5}
\colorbox[argb]{0.5, 0,0,0}{opacity = 0.5}
\colorbox[RGB]{255,255,255}{text}
\colorbox[RGBA]{255,255,255,128}{opacity = 128}
\colorbox[ARGB]{128, 255,255,255}{opacity = 128}
```

## CMYK mode

`cmyk` mode accepts four (or five with opacity) parameters in the range from 0
to 1 for the CMYK color model. Examples:

```
\colorbox[cmyk]{0,0,0,0}{text}
\colorbox[cmyk]{0,0,0,0, 0.5}{opacity = 0.5}
```

## HTML mode

`HTML` mode accepts six or eight hexadecimal digits, in the latter case last two
should be opacity. Examples:

```
\color[HTML]{FF0000}{red text}
\color[HTML]{FF000000}{fully transparent red text}
```
