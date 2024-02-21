using AgeSharp.Common;
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
                Left.Type.ValidateAssignmentFrom(Right.Type);
            }

            ValidateExpression(Right);
        }

        public override string ToString()
        {
            if (Left is not null)
            {
                return $"{Left} = {Right};";
            }
            else
            {
                return $"{Right};";
            }
        }
    }
}
