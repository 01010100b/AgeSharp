using AgeSharp.Common;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language.Types;
using Microsoft.VisualBasic;
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
        public bool IsRefAssign { get; }

        public AssignStatement(Scope scope, AccessorExpression? left, Expression right, bool is_ref_assign) : base()
        {
            Scope = scope;
            Left = left;
            Right = right;
            IsRefAssign = is_ref_assign;
        }

        public override IEnumerable<Block> GetContainedBlocks() => Enumerable.Empty<Block>();

        public override IEnumerable<Expression> GetContainedExpressions()
        {
            yield return Right;

            if (Left is not null)
            {
                yield return Left;
            }
        }

        public override void Validate()
        {
            if (Left is not null)
            {
                ValidateExpression(Left);
                Left.Type.ValidateAssignmentFrom(Right.Type);
            }
            else if (IsRefAssign)
            {
                throw new NotSupportedException($"Ref assign to nothing.");
            }

            ValidateExpression(Right);

            Throw.If<NotSupportedException>(IsRefAssign && Right is not AccessorExpression, $"Ref assign with value not accessor expression");
        }

        public override string ToString()
        {
            if (Left is not null)
            {
                return $"{Left} = {(IsRefAssign ? "ref " : "")}{Right};";
            }
            else
            {
                return $"{Right};";
            }
        }
    }
}
