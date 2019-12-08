module WpfMath.Tests.ApprovalTestUtils

open System.Globalization
open System.Text
open System.Reflection
open System.Windows.Media

open ApprovalTests
open ApprovalTests.Namers
open ApprovalTests.Reporters
open Newtonsoft.Json
open Newtonsoft.Json.Converters
open Newtonsoft.Json.Serialization

open WpfMath

[<assembly: UseReporter(typeof<DiffReporter>)>]
[<assembly: UseApprovalSubdirectory("TestResults")>]
do ()

type private InnerPropertyContractResolver() =
    inherit DefaultContractResolver()
    member private _.DoCreateProperty(p, ms) =
        base.CreateProperty(p, ms, Readable = true)

    override this.CreateProperties(``type``, memberSerialization) =
        // override that to serialize internal properties, too
        upcast ResizeArray(
            ``type``.GetProperties(BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance)
            |> Seq.filter (fun p -> Array.isEmpty <| p.GetIndexParameters()) // no indexers
            |> Seq.map (fun p -> this.DoCreateProperty(p, memberSerialization))
        )

type private GlyphTypefaceConverter() =
    inherit JsonConverter()
    override _.CanConvert ``type`` = ``type`` = typeof<GlyphTypeface>

    override _.CanRead = false
    override _.ReadJson(_, _, _, _) = failwith "Not supported"

    override _.WriteJson(writer, value, _) =
        let typeface = value :?> GlyphTypeface
        if isNull typeface then
            writer.WriteNull()
        else
            writer.WriteValue(typeface.FontUri)

/// This converter should provide the same results on both .NET 4.6.1 and .NET Core 3.0 which is important for approval
/// tests. The roundtrippable double formatting (used by default) differs between these frameworks.
type private UniversalDoubleConverter() =
    inherit JsonConverter()
    override _.CanConvert ``type`` = ``type`` = typeof<float>

    override _.CanRead = false
    override _.ReadJson(_, _, _, _) = failwith "Not supported"

    override _.WriteJson(writer, value, _) =
        let doubleValue = value :?> float
        let stringified = doubleValue.ToString("0.0###############", CultureInfo.InvariantCulture)
        writer.WriteRawValue stringified

let private jsonSettings = JsonSerializerSettings(ContractResolver = InnerPropertyContractResolver(),
                                                  Formatting = Formatting.Indented,
                                                  Converters = [|
                                                      StringEnumConverter()
                                                      GlyphTypefaceConverter()
                                                      UniversalDoubleConverter()
                                                  |])

let private serialize o =
    JsonConvert.SerializeObject(o, jsonSettings)

let verifyObject: obj -> unit =
    serialize >> Approvals.Verify

let verifyParseResult (formulaText: string): unit =
    let parser = TexFormulaParser()
    let formula = parser.Parse formulaText
    verifyObject formula

let verifyParseResultScenario (scenario: string) (formulaText: string): unit =
    use block = NamerFactory.AsEnvironmentSpecificTest(fun () -> sprintf "(%s)" scenario)
    verifyParseResult formulaText

let processSpecialChars (text: string): string =
    (StringBuilder text)
        .Replace("\\", "")
        .Replace('{', '(')
        .Replace('}', ')')
        .Replace('^', '↑')
        .ToString()
