using WpfMath.ApiText.Framework;

namespace WpfMath.ApiTest;

public class ApiDefinitionTests : ApiDefinitionTestsBase
{
    [Theory]
    [InlineData("WpfMath.net462.cs")]
    [InlineData("WpfMath.net6.0.cs")]
    public Task WpfMath(string fileName) => DoTest(fileName);

    [Theory]
    [InlineData("XamlMath.Shared.net462.cs")]
    [InlineData("XamlMath.Shared.netstandard2.0.cs")]
    [InlineData("XamlMath.Shared.net6.0.cs")]
    public Task WpfMathShared(string fileName) => DoTest(fileName);

    private static Task DoTest(string fileName) => DoTest("WpfMath.ApiTest.csproj", fileName);
}
