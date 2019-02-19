module WpfMath.Tests.ApprovalTestUtils

open System.Reflection

open ApprovalTests
open ApprovalTests.Namers
open ApprovalTests.Reporters
open Newtonsoft.Json
open Newtonsoft.Json.Converters
open Newtonsoft.Json.Serialization

open System.Text
open WpfMath

[<assembly: UseReporter(typeof<DiffReporter>)>]
[<assembly: UseApprovalSubdirectory("TestResults")>]
do ()

type private InnerPropertyContractResolver() =
    inherit DefaultContractResolver()
    member private __.DoCreateProperty(p, ms) =
        let prop = base.CreateProperty(p, ms)
        prop.Readable <- true
        prop

    override this.CreateProperties(``type``, memberSerialization) =
        // override that to serialize internal properties, too
        upcast ResizeArray(
            ``type``.GetProperties(BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance)
            |> Seq.filter (fun p -> Array.isEmpty <| p.GetIndexParameters()) // no indexers
            |> Seq.map (fun p -> this.DoCreateProperty(p, memberSerialization))
        )

// TODO[F]: Remove
type private InnerPropertyContractResolverWithoutSource() =
    inherit DefaultContractResolver()
    member private __.DoCreateProperty(p, ms) =
        let prop = base.CreateProperty(p, ms)
        prop.Readable <- true
        prop

    override this.CreateProperties(``type``, memberSerialization) =
        // override that to serialize internal properties, too
        upcast ResizeArray(
            ``type``.GetProperties(BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance)
            |> Seq.filter (fun p -> Array.isEmpty <| p.GetIndexParameters()) // no indexers
            |> Seq.filter (fun p -> p.Name <> "Source")
            |> Seq.map (fun p -> this.DoCreateProperty(p, memberSerialization))
        )

let private jsonSettings = JsonSerializerSettings(ContractResolver = InnerPropertyContractResolver(),
                                                  Formatting = Formatting.Indented,
                                                  Converters = [| StringEnumConverter() |])

let private jsonSettingsWithoutSource = JsonSerializerSettings(ContractResolver = InnerPropertyContractResolverWithoutSource(),
                                                               Formatting = Formatting.Indented,
                                                               Converters = [| StringEnumConverter() |])

let private serializeWith (jsonSettings : JsonSerializerSettings) o =
    JsonConvert.SerializeObject(o, jsonSettings)

let private verifyFormulaWith (jsonSettings : JsonSerializerSettings) (formula : TexFormula) : unit =
    let result = serializeWith jsonSettings formula
    Approvals.Verify result

let verifyFormula : TexFormula -> unit = verifyFormulaWith jsonSettings
let verifyFormulaWithoutSource : TexFormula -> unit = verifyFormulaWith jsonSettingsWithoutSource

let verifyFormulaScenario (scenario : string) (formula : TexFormula) : unit =
    use block = NamerFactory.AsEnvironmentSpecificTest(fun () -> sprintf "(%s)" scenario)
    verifyFormula formula

let private verifyParseResultWith settings formulaText =
    let parser = TexFormulaParser()
    let formula = parser.Parse formulaText
    verifyFormulaWith settings formula

let verifyParseResult(formulaText : string) : unit =
    let parser = TexFormulaParser()
    let formula = parser.Parse formulaText
    verifyFormula formula

let verifyParseResultWithoutSource : string -> unit = verifyParseResultWith jsonSettingsWithoutSource

let verifyParseResultScenario (scenario : string) (formulaText : string) : unit =
    use block = NamerFactory.AsEnvironmentSpecificTest(fun () -> sprintf "(%s)" scenario)
    verifyParseResult formulaText

let processSpecialChars(text : string) : string =
    (StringBuilder text)
        .Replace("\\", "")
        .Replace('{', '(')
        .Replace('}', ')')
        .Replace('^', '↑')
        .ToString()

