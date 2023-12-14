using AgeSharp.ScriptCompiler.Language;
using AgeSharp.ScriptCompiler.Language.Expressions;
using AgeSharp.ScriptCompiler.Language.Statements;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AgeSharp.ScriptCompiler.Compiler
{
    public static class PreCompiler
    {
        public static void Compile(Script script)
        {
            var stack = new Stack<Block>();

            foreach (var method in script.Methods)
            {
                stack.Push(method.Block);
            }

            while (stack.Count > 0)
            {
                var block = stack.Pop();

                for (int i = 0; i < block.Statements.Count; i++)
                {
                    var statement = block.Statements[i];
                    statement = ExpandStatement(script, block, statement);
                    block.Statements[i] = statement;

                    if (statement is Block b)
                    {
                        stack.Push(b);
                    }
                }
            }
        }

        private static Statement ExpandStatement(Script script, Block block, Statement statement)
        {
            if (statement is AssignStatement assign)
            {
                var left = assign.Left;
                var right = assign.Right;
                var expand_left = ShouldExpand(left);
                var expand_right = ShouldExpand(right);

                if (!expand_left && !expand_right)
                {
                    return statement;
                }

                var inner = new Block(script, block);

                if (expand_left)
                {
                    left = Expand(script, inner, left) as AccessorExpression;
                }

                if (expand_right)
                {
                    right = Expand(script, inner, right);
                }

                inner.Statements.Add(new AssignStatement(left, right));

                return inner;
            }
            else
            {
                return statement;
            }
        }

        private static bool ShouldExpand(Expression expression)
        {
            if (expression is null)
            {
                return false;
            }
            else if (expression is AccessorExpression access)
            {
                if (access.OffsetExpression is null)
                {
                    return false;
                }
                else if (access.OffsetExpression is ConstExpression)
                {
                    return false;
                }
                else if (access.OffsetExpression is AccessorExpression ao && ao.OffsetExpression is null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private static Expression Expand(Script script, Block block, Expression expression) 
        {
            if (expression is AccessorExpression access)
            {
                var v = new Variable(Guid.NewGuid().ToString(), script.Int);
                block.Scope.Variables.Add(v);
                block.Statements.Add(new AssignStatement(new AccessorExpression(v), access.OffsetExpression));
                
                return new AccessorExpression(access.Variable, new AccessorExpression(v));
            }
            else
            {
                return expression;
            }
        }
    }
}
