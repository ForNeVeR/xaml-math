using WpfMath.ApiText.Framework;

namespace AvaloniaMath.ApiTest;

[UsesVerify]
public class ApiDefinitionTests : ApiDefinitionTestsBase
{
    [Theory]
    [InlineData("AvaloniaMath.net462.cs")]
    [InlineData("AvaloniaMath.netstandard2.0.cs")]
    [InlineData("AvaloniaMath.net6.0.cs")]
    public Task AvaloniaMath(string fileName) => DoTest(fileName);

    [Theory]
    [InlineData("XamlMath.Shared.net462.cs")]
    [InlineData("XamlMath.Shared.netstandard2.0.cs")]
    [InlineData("XamlMath.Shared.net6.0.cs")]
    public Task WpfMathShared(string fileName) => DoTest(fileName);

    private static Task DoTest(string fileName) => DoTest("AvaloniaMath.ApiTest.csproj", fileName);
}
