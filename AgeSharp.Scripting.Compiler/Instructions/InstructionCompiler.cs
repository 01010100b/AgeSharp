using AgeSharp.Common;
using AgeSharp.Scripting.Compiler.Intrinsics.Math;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language.Statements;
using AgeSharp.Scripting.Language.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Type = AgeSharp.Scripting.Language.Type;

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class InstructionCompiler(Script script, Memory memory, Settings settings)
    {
        private const int ERROR_NONE = 0;
        private const int ERROR_STACKOVERFLOW = 1;

        private Script Script { get; } = script;
        private Memory Memory { get; } = memory;
        private Settings Settings { get; } = settings;
        private Dictionary<Method, LabelInstruction> MethodLabels { get; } = [];
        private LabelInstruction LabelError { get; } = new();
        private LabelInstruction LabelEnd { get; } = new();

        public List<Instruction> Compile()
        {
            var methods = new HashSet<Method>() { Script.EntryPoint! };
            var stack = new Stack<Method>();
            stack.Push(Script.EntryPoint!);

            while (stack.Count > 0)
            {
                var method = stack.Pop();

                foreach (var expression in method.GetAllBlocks().SelectMany(x => x.Statements).SelectMany(x => x.GetContainedExpressions()))
                {
                    if (expression is CallExpression call && call.Method is not Intrinsic)
                    {
                        if (!methods.Contains(call.Method))
                        {
                            methods.Add(call.Method);
                            stack.Push(call.Method);
                        }
                    }
                }
            }

            MethodLabels.Clear();

            foreach (var method in methods)
            {
                MethodLabels.Add(method, new());
            }

            var instructions = new List<Instruction>();
            var label_postinit = new LabelInstruction();

            instructions.Add(new JumpFactInstruction(Memory.MaxStackSpaceUsed, ">=", 0, label_postinit));
            instructions.AddRange(Utils.Clear(Settings.MinGoal, Settings.MaxGoal - Settings.MinGoal + 1));
            instructions.AddRange(InitializeArrays(Script.GlobalScope));

            instructions.Add(label_postinit);
            instructions.Add(new JumpFactInstruction($"up-compare-goal {Memory.Error} c:> {ERROR_NONE}", LabelError));
            
            instructions.AddRange(Utils.Clear(Memory.RegistersBase, Memory.RegistersCount));
            instructions.Add(new CommandInstruction($"up-modify-goal {Memory.StackPtr} c:= {Memory.InitialStackPtr}"));
            instructions.Add(new CommandInstruction($"up-modify-goal {Memory.ReturnAddressGoal} c:= {LabelEnd.Label}"));
            instructions.AddRange(CompileMethod(Script.EntryPoint!));

            foreach (var method in methods.Where(x => x != Script.EntryPoint))
            {
                instructions.AddRange(CompileMethod(method));
            }

            instructions.AddRange(Utils.CompileMemCpy(Memory));

            instructions.Add(LabelError);
            instructions.Add(new RuleInstruction($"up-compare-goal {Memory.Error} c:== {ERROR_STACKOVERFLOW}",
                ["chat-to-all \"ERROR: STACK OVERFLOW\""]));

            instructions.Add(LabelEnd);

            return instructions;
        }

        private List<Instruction> CompileMethod(Method method)
        {
            var instructions = new List<Instruction>();

            if (method is Intrinsic)
            {
                return instructions;
            }

            instructions.Add(MethodLabels[method]);
            instructions.Add(new CommandInstruction($"up-modify-goal {Memory.MaxStackSpaceUsed} g:max {Memory.StackPtr}"));
            instructions.Add(new RuleInstruction($"up-compare-goal {Memory.MaxStackSpaceUsed} c:> {Memory.StackLimit}",
                [
                    $"up-modify-goal {Memory.Error} c:= {ERROR_STACKOVERFLOW}",
                    $"up-jump-direct c: {LabelError.Label}"
                ]));

            instructions.AddRange(CompileBlock(method, method.Block, null, null));

            return instructions;
        }

        private List<Instruction> CompileBlock(Method method, Block block, LabelInstruction? label_break, LabelInstruction? label_continue)
        {
            var instructions = new List<Instruction>();
            instructions.AddRange(InitializeArrays(block.Scope));

            foreach (var statement in block.Statements)
            {
                if (statement is Block innerblock)
                {
                    instructions.AddRange(CompileBlock(method, innerblock, label_break, label_continue));
                }
                else if (statement is AssignStatement assign)
                {
                    var result = assign.Left is not null ? Utils.GetAddress(Memory, assign.Left) : null;

                    if (assign.IsRefAssign)
                    {
                        Throw.IfNull<NotSupportedException>(result, $"Ref assign to null target.");
                        var address = Utils.GetAddress(Memory, (AccessorExpression)assign.Right);
                        instructions.AddRange(Utils.Assign(Memory, address, result, true));
                    }
                    else
                    {
                        instructions.AddRange(CompileExpression(method, result, assign.Right));
                    }
                }
                else if (statement is IfStatement ifs)
                {
                    var label_false = new LabelInstruction();
                    var label_end = new LabelInstruction();

                    instructions.AddRange(CompileExpression(method, new Address(PrimitiveType.Bool, Memory.ConditionGoal, false), ifs.Condition));
                    instructions.Add(new JumpFactInstruction(Memory.ConditionGoal, "==", 0, label_false));
                    instructions.AddRange(CompileBlock(method, ifs.WhenTrue, label_break, label_continue));
                    instructions.Add(new JumpInstruction(label_end));
                    instructions.Add(label_false);
                    instructions.AddRange(CompileBlock(method, ifs.WhenFalse, label_break, label_continue));
                    instructions.Add(label_end);
                }
                else if (statement is LoopStatement loop)
                {
                    var label_repeat = new LabelInstruction();
                    var label_end = new LabelInstruction();
                    var label_next = new LabelInstruction();

                    instructions.AddRange(InitializeArrays(loop.Scope));
                    instructions.AddRange(CompileBlock(method, loop.Before, null, null));
                    instructions.Add(label_repeat);
                    instructions.AddRange(CompileExpression(method, new Address(PrimitiveType.Bool, Memory.ConditionGoal, false), loop.Condition));
                    instructions.Add(new JumpFactInstruction(Memory.ConditionGoal, "==", 0, label_end));
                    instructions.AddRange(CompileBlock(method, loop.Body, label_end, label_next));
                    instructions.Add(label_next);
                    instructions.AddRange(CompileBlock(method, loop.AtLoopBottom, null, null));
                    instructions.Add(new JumpInstruction(label_repeat));
                    instructions.Add(label_end);
                }
                else if (statement is ContinueStatement)
                {
                    Debug.Assert(label_continue is not null);
                    instructions.Add(new JumpInstruction(label_continue));
                }
                else if (statement is BreakStatement)
                {
                    Debug.Assert(label_break is not null);
                    instructions.Add(new JumpInstruction(label_break));
                }
                else if (statement is ReturnStatement ret)
                {
                    if (ret.Expression is not null)
                    {
                        var result = new Address(ret.Expression.Type, Memory.CallResultBase, false);
                        instructions.AddRange(CompileExpression(method, result, ret.Expression));
                    }

                    instructions.Add(new JumpIndirectInstruction(Memory.ReturnAddressGoal));
                }
                else
                {
                    throw new NotImplementedException($"Statement {statement.GetType().Name} not recognized.");
                }
            }

            return instructions;
        }

        private List<Instruction> CompileExpression(Method method, Address? result, Expression expression)
        {
            var instructions = new List<Instruction>();

            if (expression is ConstExpression constexpr)
            {
                if (result is null)
                {
                    return instructions;
                }

                instructions.AddRange(Utils.GetPointer(Memory, result, Memory.ExpressionGoal));

                if (result.Type is RefType)
                {
                    instructions.Add(new CommandInstruction($"up-get-indirect-goal g: {Memory.ExpressionGoal} {Memory.ExpressionGoal}"));
                }

                instructions.Add(new CommandInstruction($"up-set-indirect-goal g: {Memory.ExpressionGoal} c: {constexpr.Value}"));

                return instructions;
            }
            if (expression is AccessorExpression ae)
            {
                if (result is null)
                {
                    return instructions;
                }

                var address = Utils.GetAddress(Memory, ae);
                instructions.AddRange(Utils.Assign(Memory, address, result));

                return instructions;
            }
            else if (expression is CallExpression call)
            {
                if (call.Method is Intrinsic intrinsic)
                {
                    instructions.AddRange(intrinsic.Compile(Memory, result, call));

                    return instructions;
                }

                // push registers to stack
                var caller_registercount = Memory.GetRegisterCount(method);
                var callee_registercount = Memory.GetRegisterCount(call.Method);
                var registers_addr = new Address(PrimitiveType.Void, Memory.RegistersBase, false);
                var stack_addr = new Address(PrimitiveType.Void, Memory.StackPtr, true);
                var label_return = new LabelInstruction();

                instructions.AddRange(Utils.MemCpy(Memory, registers_addr, stack_addr, caller_registercount));
                instructions.AddRange(Utils.Clear(Memory.RegistersBase, callee_registercount));

                // set parameters
                instructions.Add(new CommandInstruction($"up-modify-goal {Memory.ReturnAddressGoal} c:= {label_return.Label}"));

                for (int i = 0; i < call.Method.Parameters.Count; i++)
                {
                    instructions.AddRange(AssignArgumentToParameter(call.Arguments[i], call.Method.Parameters[i]));
                }

                // increment stackptr and jump
                instructions.Add(new CommandInstruction($"up-modify-goal {Memory.StackPtr} c:+ {caller_registercount}"));
                instructions.Add(new JumpInstruction(MethodLabels[call.Method]));

                // return
                instructions.Add(label_return);
                instructions.Add(new CommandInstruction($"up-modify-goal {Memory.StackPtr} c:- {caller_registercount}"));
                instructions.AddRange(Utils.MemCpy(Memory, stack_addr, registers_addr, caller_registercount));
                
                if (result is not null)
                {
                    var call_result_addr = new Address(PrimitiveType.Void, Memory.CallResultBase, false);
                    instructions.AddRange(Utils.MemCpy(Memory, call_result_addr, result, result.Type.Size));
                }

                return instructions;
            }
            else
            {
                throw new NotImplementedException($"Expression {expression.GetType().Name} not recognized.");
            }
        }

        private List<Instruction> AssignArgumentToParameter(Expression argument, Variable parameter)
        {
            var instructions = new List<Instruction>();
            var par_addr = Memory.GetAddress(parameter);

            if (argument is ConstExpression argc)
            {
                Throw.If<NotSupportedException>(parameter.Type is RefType, $"Can not assign const to ref parameter {parameter.Name}");
                instructions.Add(new CommandInstruction($"up-modify-goal {par_addr.Goal} c:= {argc.Value}"));
            }
            else if (argument is not AccessorExpression)
            {
                throw new NotSupportedException($"Method call with parameter {parameter.Name} not const or var.");
            }

            var ae = (AccessorExpression)argument;

            var arg_addr = Utils.GetAddress(Memory, ae);
            Debug.Assert(arg_addr.IsDirect);

            if (Script.GlobalScope.Variables.Contains(ae.Variable))
            {
                instructions.AddRange(Utils.Assign(Memory, arg_addr, par_addr, true));
            }
            else
            {
                // local variables are now on stack, so interpret addr as offset from stackptr

                arg_addr = new(arg_addr.Type, Memory.StackPtr, true, arg_addr.DirectGoal - Memory.RegistersBase);
                instructions.AddRange(Utils.Assign(Memory, arg_addr, par_addr, true));

                if (parameter.Type is RefType)
                {
                    // if pointing to register, point to stack
                    var goal = par_addr.DirectGoal;
                    instructions.Add(new RuleInstruction($"up-compare-goal {goal} c:>= {Memory.RegistersBase}",
                    [
                        $"up-modify-goal {goal} c:- {Memory.RegistersBase}",
                        $"up-modify-goal {goal} g:+ {Memory.StackPtr}"
                    ]));
                }
            }

            return instructions;
        }

        private List<Instruction> InitializeArrays(Scope scope)
        {
            // first goal of array is length

            var instructions = new List<Instruction>();

            foreach (var array in scope.Variables.Where(x => x.Type is ArrayType))
            {
                var addr = Memory.GetAddress(array);
                var length = ((ArrayType)array.Type).Length;

                Debug.Assert(addr.IsDirect);
                instructions.Add(new CommandInstruction($"up-modify-goal {addr.DirectGoal} c:= {length}"));
            }

            return instructions;
        }
    }
}
