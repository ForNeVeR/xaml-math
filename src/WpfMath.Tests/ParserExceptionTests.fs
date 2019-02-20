module WpfMath.Tests.ParserExceptionTests

open Xunit

open WpfMath.Exceptions
open WpfMath.Tests.Utils

[<Fact>]
let ``Non-existing delimiter should throw exception`` () =
    let markup = @"\left x\right)"
    assertParseThrows<TexParseException> markup

[<Fact>]
let ``\mathrm{} should throw exn`` () =
    assertParseThrows<TexParseException> @"\mathrm{}"

[<Fact>]
let ``\sqrt should throw a TexParseException``() =
    assertParseThrows<TexParseException> @"\sqrt"

[<Fact>]
let ``"\sum_ " should throw a TexParseException``() =
    assertParseThrows<TexParseException> @"\sum_ "
