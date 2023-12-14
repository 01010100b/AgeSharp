using AgeSharp.ScriptCompiler.Language.Expressions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgeSharp.ScriptCompiler.Language.Statements
{
    public class AssignStatement : Statement
    {
        public AccessorExpression Left { get; }
        public Expression Right { get; }

        public AssignStatement(AccessorExpression left, Expression right)
        {
            Left = left;
            Right = right;
        }
    }
}
