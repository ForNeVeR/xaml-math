using XamlMath.ApiTest.Framework;

namespace WpfMath.ApiTest;

public class ApiDefinitionTests : ApiDefinitionTestsBase
{
    [Theory]
    [InlineData("XamlMath.Shared.net462.cs")]
    [InlineData("XamlMath.Shared.netstandard2.0.cs")]
    [InlineData("XamlMath.Shared.net8.0.cs")]
    public Task WpfMathShared(string fileName) => DoTest(fileName);

    private static Task DoTest(string fileName) => DoTest("XamlMath.Shared.ApiTest.csproj", fileName);
}
