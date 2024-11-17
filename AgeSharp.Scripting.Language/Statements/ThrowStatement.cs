namespace AgeSharp.Scripting.Language.Statements
{
    public class ThrowStatement(Scope scope, string message) : Statement
    {
        public override Scope Scope { get; } = scope;
        public string Message { get; } = message;

        public override IEnumerable<Block> GetContainedBlocks() => Enumerable.Empty<Block>();

        public override IEnumerable<Expression> GetContainedExpressions() => Enumerable.Empty<Expression>();

        public override void Validate()
        {
        }

        public override string ToString()
        {
            return $"throw new AgeException(\"{Message}\");";
        }
    }
}
