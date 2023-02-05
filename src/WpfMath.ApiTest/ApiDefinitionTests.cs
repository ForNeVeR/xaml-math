using WpfMath.ApiText.Framework;

namespace WpfMath.ApiTest;

public class ApiDefinitionTests : ApiDefinitionTestsBase
{
    [Theory]
    [InlineData("WpfMath.net452.cs")]
    [InlineData("WpfMath.netcoreapp3.1.cs")]
    public Task WpfMath(string fileName) => DoTest(fileName);

    [Theory]
    [InlineData("XamlMath.Shared.netstandard2.0.cs")]
    [InlineData("XamlMath.Shared.net452.cs")]
    [InlineData("XamlMath.Shared.netcoreapp3.1.cs")]
    public Task WpfMathShared(string fileName) => DoTest(fileName);

    private static Task DoTest(string fileName) => DoTest("WpfMath.ApiTest.csproj", fileName);
}
