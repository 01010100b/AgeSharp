namespace AgeSharp.Scripting.SharpParser
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AgeMethodAttribute : Attribute
    {
        public bool EntryPoint { get; set; } = false;
    }
}
