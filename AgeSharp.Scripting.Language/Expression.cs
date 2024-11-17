namespace AgeSharp.Scripting.Language
{
    public abstract class Expression : Validated
    {
        public abstract Type Type { get; }

        public abstract IEnumerable<Variable> GetReferencedVariables();
    }
}
