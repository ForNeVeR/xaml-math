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
        base.CreateProperty(p, ms, Readable = true)

    override this.CreateProperties(``type``, memberSerialization) =
        // override that to serialize internal properties, too
        upcast ResizeArray(
            ``type``.GetProperties(BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance)
            |> Seq.filter (fun p -> Array.isEmpty <| p.GetIndexParameters()) // no indexers
            |> Seq.map (fun p -> this.DoCreateProperty(p, memberSerialization))
        )

let private jsonSettings = JsonSerializerSettings(ContractResolver = InnerPropertyContractResolver(),
                                                  Formatting = Formatting.Indented,
                                                  Converters = [| StringEnumConverter() |])

let private serialize o =
    JsonConvert.SerializeObject(o, jsonSettings)

let verifyParseResult(formulaText : string) : unit =
    let parser = TexFormulaParser()
    let formula = parser.Parse formulaText
    let result = serialize formula
    Approvals.Verify result

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
