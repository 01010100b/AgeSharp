namespace Deimos;

class Program
{
    const string FROM = @"F:\Repos\01010100b\AgeSharp\Deimos\per";
    const string TO = @"F:\SteamLibrary\steamapps\common\AoE2DE\resources\_common\ai";

    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var result = Tests.Run();
        var per = result.GetPer();
        var file = Path.Combine(FROM, "Deimos.per");

        if (File.Exists(file))
        {
            File.Delete(file);
        }

        File.WriteAllText(file, per);

        Publish(FROM, TO);
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

        var stack = new Stack<string>();
        stack.Push(from);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            var dest = Path.Combine(to, Path.GetRelativePath(from, current));

            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
            }

            foreach (var file in Directory.EnumerateFiles(current))
            {
                var o = Path.Combine(to, Path.GetRelativePath(from, file));

                if (File.Exists(o))
                {
                    File.Delete(o);
                }

                File.Copy(file, o);
            }

            foreach (var dir in Directory.EnumerateDirectories(current))
            {
                stack.Push(dir);
            }
        }
    }
}
