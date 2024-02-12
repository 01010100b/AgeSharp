using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Statements
{
    public class AssignStatement(AccessorExpression? left, Expression right, Block block) : Statement
    {
        public override Scope Scope { get; } = block.Scope;
        public AccessorExpression? Left { get; } = left;
        public Expression Right { get; } = right;

        public override IEnumerable<Block> GetContainedBlocks() => Enumerable.Empty<Block>();

        public override void Validate()
        {
            if (Left is not null)
            {
                ValidateExpression(Left);
                Left.Type.ValidateAssignment(Right.Type, Left.Variable.IsRef);

                if (Left.Variable.IsRef && Left.Type != Right.Type)
                {
                    if (Left.Index is not null || Left.Fields is not null) throw new NotSupportedException($"Assign statement type mismatch.");
                }
            }

            ValidateExpression(Right);
        }
    }
}
