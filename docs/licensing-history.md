# Licensing History

*XAML-Math* (named *WPF-Math* at the time) was originally a direct port from the [JMathTex project][jmathtex] project written in Java, copyright 2004-2007 Universiteit Gent. *JMathTex* is licensed under the terms of the GPLv2 licence, while XAML-Math is licensed under the terms of the MIT licence.

## Current status

We got a permission from JMathTeX authors to distribute the sources of our C# port and the original resources under the terms of the MIT licence.

## Historical info

Some of the resources included in XAML-Math (`DefaultTexFont.xml`, `GlueSettings.xml`, `PredefinedTexFormulas.xml`, `TexFormulaSettings.xml`, `TexSymbols.xml`) were directly taken from the JMathTex project. To redistribute these resources under the terms of MIT license, we requested permission from the original authors of JMathTeX. Here is a fragment of our email to the JMathTeX authors (some irrelevant details from the emails are omitted because they were discussing additional technical topics concerning how the resources were initially generated):

> **FROM**: friedrich@fornever.me
>
> **TO**: Prof. Coolsaet
>
> **TO**: Prof. Van Cleemput
>
> Dear Prof. Coolsaet,
>
> Dear Prof. Van Cleemput,
>
> My name is Friedrich von Never, I am the current maintainer of WPF-Math library, an open-source reimplementation of your nice JMathTeX library.
>
> I've recently taken the project maintainership, and now I'm sorting out various licensing issues.
>
> One issue is that our project is licensed under the permissive MIT license, but we're using direct copies of some XML resources from JMathTeX (licensed under the GPL). So, we cannot redistribute these resources under the MIT license (and aren't distributing them currently). Namely these resources are: DefaultTexFont.xml, GlueSettings.xml, PredefinedTexFormulas.xml, TexFormulaSettings.xml, and TexSymbols.xml.
>
> May I ask you to give us the permission to redistribute these files under the MIT? The contributors and users of the project will greatly appreciate that.
>
> [...]

In his reply, Prof. Coolsaet kindly granted us a permission:

> **FROM**: Kris Coolsaet
>
> **TO**: friedrich@fornever.me
>
> [...]
>
> We have no problem with you redistributing the XML resource files from
> JMathTeX under the MIT license.
>
> The first version of JMathTeX was built in 2004/2005 by Kurt Vermeulen
> for his master thesis project. I can no longer reach him, but because I
> was his supervisor a the time our university policy allows me to make
> this decision in his place.
>
> [...]

We have also requested permission to distribute our C# port under the terms of the MIT licence:

> **FROM**: friedrich@fornever.me
>
> **TO**: Prof. Coolsaet
>
> [...]
>
> Having said all of that, I, as the main maintainer of the library, kindly ask you to permit the redistribution of our variant of JMathTeX source code converted to the C# programming language under the terms of the MIT license. Many people rely on the library in their scientific applications (we have 100s of downloads for each published version), and we're very much in trouble right now because of the licensing issues.

Here's the reply of Prof. Coolsaet:

> **FROM**: Kris Coolsaet
>
> **TO**: friedrich@fornever.me
>
> We have no problem with you redistributing your variant of JMathTeX (as
> mentioned above) under the terms of the MIT License.
>
> As I mentioned in our previous email conversation about JMathTeX, the
> first version of JMathTeX was built in 2004/2005 by Kurt Vermeulen for
> his master thesis project. I can no longer reach him, but because I was
> his supervisor a the time our university policy allows me to make this
> decision in his place.

If you need access to the actual email headers of these messages to verify their validity, please contact the maintainer of this project directly at friedrich@fornever.me, or Prof. Coolsaet himself (please find contacts on [JMathTeX site][jmathtex]).

[jmathtex]: https://jmathtex.sourceforge.net/
