using AgeSharp.Scripting.Language.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Statements
{
    public class IfStatement(Expression condition, Block block) : Statement
    {
        public override Scope Scope { get; } = block.Scope;
        public Expression Condition { get; } = condition;
        public Block WhenTrue { get; } = new(block.Scope);
        public Block WhenFalse { get; } = new(block.Scope);

        public override IEnumerable<Block> GetContainedBlocks()
        {
            yield return WhenTrue;
            yield return WhenFalse;
        }

        public override void Validate()
        {
            if (Condition.Type is null || Condition.Type != PrimitiveType.Bool) throw new NotSupportedException($"If condition does not have type Bool.");
            
            ValidateExpression(Condition);
        }
    }
}
