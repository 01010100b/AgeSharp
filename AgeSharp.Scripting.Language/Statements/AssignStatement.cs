using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Statements
{
    public class AssignStatement : Statement
    {
        public override Scope Scope { get; }
        public AccessorExpression? Left { get; }
        public Expression Right { get; }

        public AssignStatement(Scope scope, AccessorExpression? left, Expression right) : base()
        {
            Scope = scope;
            Left = left;
            Right = right;
        }

        public override IEnumerable<Block> GetContainedBlocks() => Enumerable.Empty<Block>();

        public override void Validate()
        {
            if (Left is not null)
            {
                ValidateExpression(Left);
                Right.Type.ValidateAssignment(Left.Type, Left.Variable.IsRef);

                if (Left.Variable.IsRef && Left.Type != Right.Type)
                {
                    if (Left.Index is not null || Left.Fields is not null) throw new NotSupportedException($"Assign statement type mismatch.");
                }
            }

            ValidateExpression(Right);
        }
    }
}
