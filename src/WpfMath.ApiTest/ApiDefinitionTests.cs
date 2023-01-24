using System.Reflection;
using DiffEngine;

namespace WpfMath.ApiTest;

[UsesVerify]
public class ApiDefinitionTests
{
    static ApiDefinitionTests()
    {
        DiffRunner.Disabled = true;
    }

    [Theory]
    [InlineData("WpfMath.net452.cs")]
    [InlineData("WpfMath.netcoreapp3.1.cs")]
    public Task WpfMath(string fileName)
    {
        var directory = GetProjectDirectory();
        var goldFile = Path.Combine(directory, "api", fileName);
        var sourceFile = Path.ChangeExtension(goldFile, "tmp.cs");
        return VerifyFile(sourceFile)
            .UseDirectory(Path.GetDirectoryName(goldFile)!)
            .UseFileName(Path.GetFileNameWithoutExtension(goldFile));
    }

    private static string GetProjectDirectory()
    {
        var assemblyLocation =Assembly.GetExecutingAssembly().Location;
        var currentLocation = assemblyLocation;
        const string csProjFileName = "WpfMath.ApiTest.csproj";

        while (true)
        {
            currentLocation = Path.GetDirectoryName(currentLocation);
            if (currentLocation is null)
                throw new Exception(
                    $"""Cannot find file "{csProjFileName}" in any parents of directory "{Path.GetDirectoryName(assemblyLocation)}".""");

            var csProjFile = Path.Combine(currentLocation, csProjFileName);
            if (File.Exists(csProjFile)) return currentLocation;
        }
    }
}
