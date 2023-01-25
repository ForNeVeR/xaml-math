module WpfMath.Tests.Utils

open System
open System.Windows

open Xunit

open WpfMath.Parsers

let initializeFontResourceLoading =
    let monitor = obj()
    fun () ->
        lock monitor (fun () ->
            if not(UriParser.IsKnownScheme "pack")
            then new Application() |> ignore)

let assertParseThrows<'ex when 'ex :> exn>(formula: string): 'ex =
    let parser = WpfTeXFormulaParser.Instance
    Assert.Throws<'ex>(Func<obj>(fun () -> upcast parser.Parse formula))
