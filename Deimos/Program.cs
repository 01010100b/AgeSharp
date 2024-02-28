using AgeSharp.Scripting.Compiler;
using AgeSharp.Scripting.SharpParser;
using System.Diagnostics;
using System.Text;

namespace Deimos;

class Program
{
    const string SOURCE = @"F:\Repos\01010100b\AgeSharp\Deimos\Source";
    const string FROM = @"F:\Repos\01010100b\AgeSharp\Deimos\per";
    const string TO = @"F:\SteamLibrary\steamapps\common\AoE2DE\resources\_common\ai";

    static void Main(string[] args)
    {
        var result = GetCompilation(SOURCE);
        var per = result.GetPer();

        var sb = new StringBuilder();
        sb.AppendLine("(include \"..\\ai\\Deimos.xs\")");
        sb.AppendLine();
        sb.AppendLine(per);
        per = sb.ToString();

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
        if (!Directory.Exists(to))
        {
            Directory.CreateDirectory(to);
        }

        var name = Path.GetFileNameWithoutExtension(Directory.EnumerateFiles(from, "*.per").Single());
        var ai = Path.Combine(to, name + ".ai");

        if (!File.Exists(ai))
        {
            File.Create(ai);
        }

        foreach (var file in Directory.EnumerateFiles(from, "*.*"))
        {
            var dest = Path.Combine(to, Path.GetRelativePath(from, file));

            if (File.Exists(dest))
            {
                File.Delete(dest);
            }

            File.Copy(file, dest);
        }

        var dirs = new Stack<string>();

        foreach (var dir in Directory.EnumerateDirectories(from))
        {
            dirs.Push(dir);
        }

        while (dirs.Count > 0)
        {
            var dir = dirs.Pop();
            var todir = Path.Combine(to, Path.GetRelativePath(from, dir));

            if (Directory.Exists(todir))
            {
                Directory.Delete(todir, true);
            }

            Directory.CreateDirectory(todir);

            foreach (var file in Directory.EnumerateFiles(dir, "*.*"))
            {
                var dest = Path.Combine(todir, Path.GetRelativePath(dir, file));
                File.Copy(file, dest, true);
            }

            foreach (var subdir in Directory.EnumerateDirectories(dir))
            {
                dirs.Push(subdir);
            }
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
