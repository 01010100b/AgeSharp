namespace AgeSharp.Scripting.Language
{
    public abstract class Validated
    {
        public static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new Exception($"Invalid name {name}.");
        }

        public abstract void Validate();
    }
}
