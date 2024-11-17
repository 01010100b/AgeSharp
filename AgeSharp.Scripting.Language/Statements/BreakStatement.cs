namespace AgeSharp.Scripting.Language.Statements
{
    public class BreakStatement(Scope scope) : Statement
    {
        public override Scope Scope { get; } = scope;

        public override IEnumerable<Block> GetContainedBlocks() => Enumerable.Empty<Block>();

        public override IEnumerable<Expression> GetContainedExpressions() => Enumerable.Empty<Expression>();

        public override void Validate()
        {
        }

        public override string ToString()
        {
            return "break;";
        }
    }
}
