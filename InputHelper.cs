using System.Runtime.CompilerServices;

namespace adventOfCode;

public static class InputHelper
{
    public static string GetInput([CallerFilePath] string callerFilePath = "")
    {
        var inputPath = GetInputPath(callerFilePath);

        return File.ReadAllText(inputPath);
    }
    
    public static string[] GetInputLines([CallerFilePath] string callerFilePath = "")
    {
        var inputPath = GetInputPath(callerFilePath);

        return File.ReadAllLines(inputPath);
    }
    
    public static IEnumerable<string> ReadInputLines([CallerFilePath] string callerFilePath = "")
    {
        var inputPath = GetInputPath(callerFilePath);

        return File.ReadLines(inputPath);
    }

    private static string GetInputPath(string callerFilePath)
    {
        var binIndex = AppDomain.CurrentDomain.BaseDirectory.IndexOf("/bin", StringComparison.Ordinal);
        var projectPath = AppDomain.CurrentDomain.BaseDirectory[..(binIndex+1)];
        
        var @base = new Uri(projectPath);
        var caller = new Uri(callerFilePath);
         
        var relativePath = @base.MakeRelativeUri(caller).ToString();

        return $"{relativePath[..relativePath.LastIndexOf('/')]}/in.input";
    }
}