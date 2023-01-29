using System.Reflection;
using DiffEngine;

namespace WpfMath.ApiText.Framework;

[UsesVerify]
public class ApiDefinitionTestsBase
{
    static ApiDefinitionTestsBase()
    {
        DiffRunner.Disabled = true;
    }

    protected static Task DoTest(string csProjFileName, string fileName)
    {
        var directory = GetProjectDirectory(csProjFileName);
        var goldFile = Path.Combine(directory, "../../api", fileName);
        var sourceFile = Path.ChangeExtension(goldFile, "tmp.cs");
        return VerifyFile(sourceFile)
            .UseDirectory(Path.GetDirectoryName(goldFile)!)
            .UseFileName(Path.GetFileNameWithoutExtension(goldFile));
    }

    protected static string GetProjectDirectory(string csProjFileName)
    {
        var assemblyLocation =Assembly.GetExecutingAssembly().Location;
        var currentLocation = assemblyLocation;

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
