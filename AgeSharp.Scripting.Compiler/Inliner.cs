using AgeSharp.Common;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language.Statements;
using AgeSharp.Scripting.Language.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler
{
    internal static class Inliner
    {
        public static void Inline(Script script, Settings settings)
        {
            for (int i = 0; i < 3; i++)
            {
                InlinePass(script, settings);
            }
        }

        private static void InlinePass(Script script, Settings settings)
        {
            var variables = new Dictionary<Variable, Variable>();

            foreach (var method in script.Methods)
            {
                if (method is Intrinsic)
                {
                    continue;
                }

                variables.Clear();

                foreach (var variable in script.GlobalScope.Variables)
                {
                    variables.Add(variable, variable);
                }

                foreach (var variable in method.GetAllBlocks().SelectMany(x => x.Scope.Variables))
                {
                    variables.Add(variable, variable);
                }

                foreach (var block in method.GetAllBlocks())
                {
                    for (int i = 0; i < block.Statements.Count; i++)
                    {
                        var statement = block.Statements[i];
                        var skip = new Block(block.Scope);

                        if (statement is AssignStatement assign)
                        {
                            if (assign.Right is not CallExpression call)
                            {
                                continue;
                            }

                            if (call.Method is Intrinsic)
                            {
                                continue;
                            }

                            if (GetScore(call.Method) > settings.InlineThreshold)
                            {
                                continue;
                            }

                            Inline(call, skip, assign.Left, variables);
                            block.Statements[i] = skip;
                        }
                    }
                }
            }
        }

        private static double GetScore(Method method)
        {
            if (method is Intrinsic)
            {
                return double.MaxValue;
            }

            var score = 0d;

            foreach (var block in method.GetAllBlocks())
            {
                score += 0.1;

                foreach (var statement in block.Statements)
                {
                    if (statement is BreakStatement || statement is ContinueStatement || statement is SkipStatement)
                    {
                        score += 0.1;
                    }
                    else if (statement is IfStatement)
                    {
                        score += 2;
                    }
                    else if (statement is LoopStatement)
                    {
                        score += 3;
                    }
                    else
                    {
                        score += 1;
                    }
                }
            }

            return score;
        }

        private static void Inline(CallExpression call, Block block, AccessorExpression? result, Dictionary<Variable, Variable> variables)
        {
            Copy(call.Method.Block, block, block, result, variables, []);

            for (int i = 0; i < call.Arguments.Count; i++)
            {
                var arg = call.Arguments[i];
                var par = variables[call.Method.Parameters[i]];
                block.Statements.Insert(0, new AssignStatement(block.Scope, new AccessorExpression(par), arg, par.Type is RefType));
            }
        }

        private static void Copy(Block from, Block to, Block skip, AccessorExpression? result, Dictionary<Variable, Variable> variables, Dictionary<Block, Block> blocks)
        {
            blocks.Add(from, to);

            foreach (var variable in from.Scope.Variables)
            {
                var v = new Variable($"varinl-{Guid.NewGuid()}", variable.Type);
                variables.Add(variable, v);
                to.Scope.AddVariable(v);
            }

            foreach (var statement in from.Statements)
            {
                if (statement is BreakStatement)
                {
                    to.Statements.Add(new BreakStatement(to.Scope));
                }
                else if (statement is ContinueStatement)
                {
                    to.Statements.Add(new ContinueStatement(to.Scope));
                }
                else if (statement is ThrowStatement thrst)
                {
                    to.Statements.Add(new ThrowStatement(to.Scope, thrst.Message));
                }
                else if (statement is AssignStatement assign)
                {
                    var left = assign.Left is not null ? (AccessorExpression)Copy(assign.Left, variables) : null;
                    var right = Copy(assign.Right, variables);
                    to.Statements.Add(new AssignStatement(to.Scope, left, right, assign.IsRefAssign));
                }
                else if (statement is ReturnStatement retst)
                {
                    var expr = retst.Expression is not null ? Copy(retst.Expression, variables) : null;

                    if (expr is not null && result is not null)
                    {
                        var left = (AccessorExpression)Copy(result, variables);
                        to.Statements.Add(new AssignStatement(to.Scope, left, expr, false));
                    }

                    to.Statements.Add(new SkipStatement(to.Scope, skip));
                }
                else if (statement is SkipStatement skipst)
                {
                    var block = blocks[skipst.Block];
                    to.Statements.Add(new SkipStatement(to.Scope, block));
                }
                else if (statement is IfStatement ifst)
                {
                    var condition = Copy(ifst.Condition, variables);
                    var ifs = new IfStatement(to.Scope, condition);
                    Copy(ifst.WhenTrue, ifs.WhenTrue, skip, result, variables, blocks);
                    Copy(ifst.WhenFalse, ifs.WhenFalse, skip, result, variables, blocks);
                    to.Statements.Add(ifs);
                }
                else if (statement is LoopStatement loopst)
                {
                    var scoping_block = new Block(to.Scope);
                    Copy(loopst.ScopingBlock, scoping_block, skip, result, variables, blocks);
                    var condition = Copy(loopst.Condition, variables);
                    var before = new Block(scoping_block.Scope);
                    Copy(loopst.Before, before, skip, result, variables, blocks);
                    var body = new Block(scoping_block.Scope);
                    Copy(loopst.Body, body, skip, result, variables, blocks);
                    var at_loop_bottom = new Block(scoping_block.Scope);
                    Copy(loopst.AtLoopBottom, at_loop_bottom, skip, result, variables, blocks);
                    var loops = new LoopStatement(scoping_block, condition, before, body, at_loop_bottom, loopst.ConditionAtTop);
                    to.Statements.Add(loops);
                }
                else if (statement is Block blst)
                {
                    var bst = new Block(to.Scope);
                    Copy(blst, bst, skip, result, variables, blocks);
                    to.Statements.Add(bst);
                }
                else
                {
                    throw new NotImplementedException($"Statement {statement.GetType().Name} not recognized.");
                }
            }
        }

        private static Expression Copy(Expression expression, Dictionary<Variable, Variable> variables)
        {
            if (expression is ConstExpression constexpr)
            {
                return new ConstExpression(constexpr.Type, constexpr.Value);
            }
            else if (expression is AccessorExpression access)
            {
                var v = variables[access.Variable];

                if (access.IsVariableAccess)
                {
                    return new AccessorExpression(v);
                }
                else if (access.IsStructAccess)
                {
                    return new AccessorExpression(v, access.Fields!);
                }
                else if (access.IsArrayAccess)
                {
                    var index = Copy(access.Index!, variables);

                    return new AccessorExpression(v, index);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            else if (expression is CallExpression call)
            {
                var c = new CallExpression(call.Method, call.Literal);

                foreach (var arg in call.Arguments)
                {
                    c.AddArgument(Copy(arg, variables));
                }

                return c;
            }
            else
            {
                throw new NotImplementedException($"Expression {expression.GetType().Name} not recognized.");
            }
        }
    }
}
