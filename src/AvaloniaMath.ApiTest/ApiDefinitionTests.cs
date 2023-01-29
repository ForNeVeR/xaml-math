using WpfMath.ApiText.Framework;

namespace AvaloniaMath.ApiTest;

[UsesVerify]
public class ApiDefinitionTests : ApiDefinitionTestsBase
{
    [Theory]
    [InlineData("AvaloniaMath.netstandard2.0.cs")]
    [InlineData("AvaloniaMath.netstandard2.1.cs")]
    public Task AvaloniaMath(string fileName) => DoTest(fileName);

    [Theory]
    [InlineData("WpfMath.Shared.net452.cs")]
    [InlineData("WpfMath.Shared.netcoreapp3.1.cs")]
    public Task WpfMathShared(string fileName) => DoTest(fileName);

    private static Task DoTest(string fileName) => DoTest("AvaloniaMath.ApiTest.csproj", fileName);
}
