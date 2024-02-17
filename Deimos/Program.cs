using AgeSharp.Scripting.Compiler;
using AgeSharp.Scripting.SharpParser;
using System.Diagnostics;

namespace Deimos;

class Program
{
    const string SOURCE = @"F:\Repos\01010100b\AgeSharp\Deimos\Source";
    const string FROM = @"F:\Repos\01010100b\AgeSharp\Deimos\per";
    const string TO = @"F:\SteamLibrary\steamapps\common\AoE2DE\resources\_common\ai";

    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var result = GetCompilation(SOURCE);
        var per = result.GetPer();
        var file = Path.Combine(FROM, "Deimos.per");

        if (File.Exists(file))
        {
            File.Delete(file);
        }

        File.WriteAllText(file, per);

        Publish(FROM, TO);
        OpenDebugFile(result);
    }

    private static CompilationResult GetCompilation(string folder)
    {
        var sources = new List<string>();

        foreach (var file in Directory.EnumerateFiles(folder, "*.cs", SearchOption.AllDirectories))
        {
            sources.Add(File.ReadAllText(file));
        }

        var parser = new Parser();
        var script = parser.Parse(sources);
        var compiler = new ScriptCompiler();
        var result = compiler.Compile(script, new());

        return result;
    }

    private static void Publish(string from, string to)
    {
        var name = "";

        foreach (var file in Directory.EnumerateFiles(from, "*.per"))
        {
            name = Path.GetFileNameWithoutExtension(file);
        }

        if (string.IsNullOrEmpty(name))
        {
            throw new Exception("Per not found.");
        }

        var ai = Path.Combine(to, name + ".ai");

        if (!File.Exists(ai))
        {
            File.Create(ai);
        }

        foreach (var file in Directory.EnumerateFiles(from, "*.*", SearchOption.AllDirectories))
        {
            var dest = Path.Combine(to, Path.GetRelativePath(from, file));

            if (File.Exists(dest))
            {
                File.Delete(dest);
            }

            File.Copy(file, dest);
        }
    }

    private static void OpenDebugFile(CompilationResult result)
    {
        var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug.txt");

        if (File.Exists(file))
        {
            File.Delete(file);
        }
        
        File.WriteAllText(file, result.ToString());
        var psinfo = new ProcessStartInfo() { FileName = file, UseShellExecute = true };
        Process.Start(psinfo);
    }
}
