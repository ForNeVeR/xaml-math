using XamlMath.ApiTest.Framework;

namespace AvaloniaMath.ApiTest;

public class ApiDefinitionTests : ApiDefinitionTestsBase
{
    [Theory]
    [InlineData("AvaloniaMath.net462.cs")]
    [InlineData("AvaloniaMath.netstandard2.0.cs")]
    [InlineData("AvaloniaMath.net8.0.cs")]
    public Task AvaloniaMath(string fileName) => DoTest(fileName);

    [Theory]
    [InlineData("XamlMath.Shared.net462.cs")]
    [InlineData("XamlMath.Shared.netstandard2.0.cs")]
    [InlineData("XamlMath.Shared.net8.0.cs")]
    public Task WpfMathShared(string fileName) => DoTest(fileName);

    private static Task DoTest(string fileName) => DoTest("AvaloniaMath.ApiTest.csproj", fileName);
}
