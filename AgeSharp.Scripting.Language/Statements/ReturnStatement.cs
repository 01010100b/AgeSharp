using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override void Validate()
        {
            if (Expression is not null)
            {
                ValidateExpression(Expression);
            }
        }
    }
}
