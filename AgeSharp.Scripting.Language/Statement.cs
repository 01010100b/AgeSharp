using AgeSharp.Scripting.Language.Statements;

namespace AgeSharp.Scripting.Language
{
    public abstract class Statement : Validated
    {
        public abstract Scope Scope { get; }
        public abstract IEnumerable<Block> GetContainedBlocks();
        public abstract IEnumerable<Expression> GetContainedExpressions();

        protected void ValidateExpression(Expression expression)
        {
            expression.Validate();

            foreach (var variable in expression.GetReferencedVariables())
            {
                if (!Scope.IsInScope(variable))
                {
                    throw new NotSupportedException($"Variable {variable} is not in scope.");
                }
            }
        }
    }
}
