﻿using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language.Statements;
using AgeSharp.Scripting.Language.Types;
using System.Diagnostics;

namespace AgeSharp.Scripting.Compiler
{
    internal static class Transformer
    {
        // array indices and call arguments are at most direct variable accesses

        public static void Transform(Script script, Settings settings)
        {
            foreach (var method in script.Methods.Where(x => x is not Intrinsic))
            {
                var changed = true;

                while (changed)
                {
                    changed = false;

                    foreach (var block in method.GetAllBlocks())
                    {
                        changed |= TransformBlock(script, block);
                    }
                }
            }
        }

        private static bool TransformBlock(Script script, Block block)
        {
            var changed = false;

            for (int i = 0; i < block.Statements.Count; i++)
            {
                var statement = block.Statements[i];

                if (!NeedsStatementTransform(statement))
                {
                    continue;
                }

                var bb = new Block(block.Scope);
                TransformStatement(script, statement, bb);
                block.Statements[i] = bb;
                changed = true;
            }

            return changed;
        }

        private static void TransformStatement(Script script, Statement statement, Block block)
        {
            static void move(Block from, Block to)
            {
                foreach (var statement in from.Statements)
                {
                    foreach (var block in statement.GetContainedBlocks())
                    {
                        if (block.Scope.Parent == from.Scope)
                        {
                            block.Scope.Rebase(to.Scope);
                        }
                    }

                    to.Statements.Add(statement);
                }

                foreach (var variable in from.Scope.Variables)
                {
                    to.Scope.AddVariable(variable);
                }
            }

            if (statement is AssignStatement assign)
            {
                var left = assign.Left is not null ? (AccessorExpression)TransformExpression(script, assign.Left, block) : null;
                var right = TransformExpression(script, assign.Right, block);
                var st = new AssignStatement(block.Scope, left, right, assign.IsRefAssign);
                block.Statements.Add(st);
            }
            else if (statement is IfStatement ifs)
            {
                var condition = TransformExpression(script, ifs.Condition, block);
                var st = new IfStatement(block.Scope, condition);
                move(ifs.WhenTrue, st.WhenTrue);
                move(ifs.WhenFalse, st.WhenFalse);
                block.Statements.Add(st);
            }
            else if (statement is LoopStatement loop)
            {
                var condition = TransformExpression(script, loop.Condition, block);
                var scoping_block = new Block(block.Scope);
                var before = new Block(scoping_block.Scope);
                var body = new Block(scoping_block.Scope);
                var atloopbottom = new Block(scoping_block.Scope);

                move(loop.ScopingBlock, scoping_block);
                move(loop.Before, before);
                move(loop.Body, body);
                move(loop.AtLoopBottom, atloopbottom);

                var st = new LoopStatement(scoping_block, condition, before, body, atloopbottom, loop.ConditionAtTop); ;
                block.Statements.Add(st);
            }
            else if (statement is ReturnStatement ret)
            {
                var expression = TransformExpression(script, ret.Expression!, block);
                var st = new ReturnStatement(block.Scope, expression);
                block.Statements.Add(st);
            }
            else
            {
                throw new NotImplementedException($"Statement {statement.GetType().Name} not recognized.");
            }
        }

        private static bool NeedsStatementTransform(Statement statement)
        {
            if (statement is AssignStatement assign)
            {
                if (NeedsExpressionTransform(assign.Right))
                {
                    return true;
                }
                else if (assign.Left is not null && NeedsExpressionTransform(assign.Left))
                {
                    return true;
                }
            }
            else if (statement is IfStatement ifs)
            {
                if (NeedsExpressionTransform(ifs.Condition))
                {
                    return true;
                }
            }
            else if (statement is LoopStatement loop)
            {
                if (NeedsExpressionTransform(loop.Condition))
                {
                    return true;
                }
            }
            else if (statement is ReturnStatement ret)
            {
                if (ret.Expression is not null && NeedsExpressionTransform(ret.Expression))
                {
                    return true;
                }
            }

            return false;
        }

        private static Expression TransformExpression(Script script, Expression expression, Block block)
        {
            if (!NeedsExpressionTransform(expression))
            {
                return expression;
            }

            if (expression is AccessorExpression access)
            {
                Debug.Assert(access.Index is not null);
                var vi = new Variable("var-" + Guid.NewGuid().ToString(), PrimitiveType.Int);
                block.Scope.AddVariable(vi);
                block.Statements.Add(new AssignStatement(block.Scope, new AccessorExpression(vi), access.Index!, false));

                return new AccessorExpression(access.Variable, new AccessorExpression(vi));
            }
            else if (expression is CallExpression call)
            {
                var cn = new CallExpression(call.Method, call.Literal);

                for (int i = 0; i < call.Arguments.Count; i++)
                {
                    var arg = call.Arguments[i];
                    var ref_par = call.Method.Parameters[i].Type is RefType;

                    if (!NeedsArgumentTransform(arg, ref_par))
                    {
                        cn.AddArgument(arg);

                        continue;
                    }

                    if (arg is AccessorExpression ae)
                    {
                        if (ref_par)
                        {
                            Debug.Assert(!ae.IsVariableAccess);
                            var type = ae.Type is RefType ? ae.Type : script.GetRefType(ae.Type);
                            var va = new Variable("var-" + Guid.NewGuid().ToString(), type);
                            block.Scope.AddVariable(va);
                            block.Statements.Add(new AssignStatement(block.Scope, new AccessorExpression(va), ae, true));
                            cn.AddArgument(new AccessorExpression(va));
                        }
                        else
                        {
                            Debug.Assert(ae.IsArrayAccess);
                            Debug.Assert(ae.Index is not null);

                            if (ae.Index is AccessorExpression ai && ai.IsVariableAccess)
                            {
                                var type = ae.Type;
                                var va = new Variable("var-" + Guid.NewGuid().ToString(), type);
                                block.Scope.AddVariable(va);
                                block.Statements.Add(new AssignStatement(block.Scope, new AccessorExpression(va), ae, false));
                                cn.AddArgument(new AccessorExpression(va));
                            }
                            else
                            {
                                var vi = new Variable("var-" + Guid.NewGuid().ToString(), PrimitiveType.Int);
                                block.Scope.AddVariable(vi);
                                block.Statements.Add(new AssignStatement(block.Scope, new AccessorExpression(vi), ae.Index!, false));
                                cn.AddArgument(new AccessorExpression(ae.Variable, new AccessorExpression(vi)));
                            }
                        }
                    }
                    else
                    {
                        var va = new Variable("var-" + Guid.NewGuid().ToString(), arg.Type);
                        block.Scope.AddVariable(va);
                        block.Statements.Add(new AssignStatement(block.Scope, new AccessorExpression(va), arg, false));
                        cn.AddArgument(new AccessorExpression(va));
                    }
                }

                return cn;
            }
            else
            {
                throw new NotImplementedException($"Expression {expression.GetType().Name} not recognized.");
            }
        }

        private static bool NeedsExpressionTransform(Expression expression)
        {
            if (expression is AccessorExpression access && access.IsArrayAccess)
            {
                if (access.Index is AccessorExpression ae)
                {
                    if (!ae.IsVariableAccess)
                    {
                        return true;
                    }
                }
                else if (access.Index is not ConstExpression)
                {
                    return true;
                }
            }
            else if (expression is CallExpression call)
            {
                for (int i = 0; i < call.Arguments.Count; i++)
                {
                    var arg = call.Arguments[i];

                    if (NeedsArgumentTransform(arg, call.Method.Parameters[i].Type is RefType))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool NeedsArgumentTransform(Expression arg, bool ref_par = false)
        {
            if (arg is AccessorExpression arga)
            {
                if (ref_par && !arga.IsVariableAccess)
                {
                    return true;
                }
                else if (arga.IsArrayAccess)
                {
                    if (arga.Index is not ConstExpression)
                    {
                        return true;
                    }
                }
            }
            else if (arg is not ConstExpression)
            {
                return true;
            }

            return false;
        }
    }
}
