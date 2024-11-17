namespace AgeSharp.Scripting.Language.Statements
{
    public class ReturnStatement : Statement
    {
        public override Scope Scope { get; }
        public Expression? Expression { get; }

        public ReturnStatement(Scope scope, Expression? expression) : base()
        {
            Scope = scope;
            Expression = expression;
        }

        public override IEnumerable<Block> GetContainedBlocks() => Enumerable.Empty<Block>();

        public override IEnumerable<Expression> GetContainedExpressions()
        {
            if (Expression is not null)
            {
                yield return Expression;
            }
        }

        public override void Validate()
        {
            if (Expression is not null)
            {
                ValidateExpression(Expression);
            }
        }

        public override string ToString()
        {
            return Expression is null ? "return;" : $"return {Expression};";
        }
    }
}
