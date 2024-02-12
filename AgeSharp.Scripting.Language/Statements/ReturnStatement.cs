using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Statements
{
    public class ReturnStatement(Expression? expression, Block block) : Statement
    {
        public override Scope Scope { get; } = block.Scope;
        public Expression? Expression { get; } = expression;

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
