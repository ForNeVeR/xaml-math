module WpfMath.Tests.Utils

open System
open System.Windows

open Xunit

open WpfMath

let textStyle = "text"
let rmStyle = "mathrm"
let itStyle = "mathit"
let calStyle = "mathcal"

let initializeFontResourceLoading =
    let monitor = obj()
    fun () ->
        lock monitor (fun () ->
            if not(UriParser.IsKnownScheme "pack")
            then new Application() |> ignore)

let assertParseThrows<'ex when 'ex :> exn> formula =
    let parser = TexFormulaParser()
    Assert.Throws<'ex>(Func<obj>(fun () -> upcast parser.Parse(formula)))

// TODO[F]: Move to BoxTests because it's used only there
let src (string : string) (start : int) (len : int) = SourceSpan(string, start, len)

let private toNullable = function
    | Some x -> Nullable x
    | None -> Nullable()
