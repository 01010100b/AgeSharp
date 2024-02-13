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

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal class InstructionCompiler(Script script, Memory memory, Settings settings)
    {
        private Script Script { get; } = script;
        private Memory Memory { get; } = memory;
        private Settings Settings { get; } = settings;
        private Dictionary<Method, LabelInstruction> MethodLabels { get; } = [];
        private LabelInstruction LabelEnd { get; } = new();

        public List<Instruction> Compile()
        {
            MethodLabels.Clear();

            foreach (var method in Script.Methods.Where(x => x is not Intrinsic))
            {
                MethodLabels.Add(method, new());
            }

            var instructions = new List<Instruction>();
            var label_postinit = new LabelInstruction();

            instructions.Add(new JumpFactInstruction(Memory.MaxStackSpaceUsed, ">=", 0, label_postinit));
            instructions.AddRange(Utils.Clear(Settings.MinGoal, Settings.MaxGoal - Settings.MinGoal + 1));
            instructions.AddRange(InitializeArrays(Script.GlobalScope));

            instructions.Add(label_postinit);
            instructions.AddRange(Utils.Clear(Memory.RegisterBase, Memory.RegisterCount));
            instructions.Add(new CommandInstruction($"up-modify-goal {Memory.StackPtr} c:= {Memory.InitialStackPtr}"));
            instructions.Add(new CommandInstruction($"up-modify-goal {Memory.RegisterBase} c:= {LabelEnd.Label}"));

            instructions.AddRange(CompileMethod(Script.EntryPoint!));

            foreach (var method in Script.Methods.Where(x => x != Script.EntryPoint))
            {
                instructions.AddRange(CompileMethod(method));
            }

            instructions.AddRange(Utils.CompileMemCpy(Memory));
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
                    var result = assign.Left is not null ? GetAddress(assign.Left) : null;

                    instructions.AddRange(CompileExpression(method, result, assign.Right));
                }
                else if (statement is IfStatement ifs)
                {
                    var label_false = new LabelInstruction();
                    var label_end = new LabelInstruction();

                    instructions.AddRange(CompileExpression(method, new Address(Memory.ConditionGoal, false), ifs.Condition));
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

                    instructions.AddRange(InitializeArrays(loop.Scope));
                    instructions.AddRange(CompileBlock(method, loop.Prefix, null, null));
                    instructions.Add(label_repeat);
                    instructions.AddRange(CompileExpression(method, new Address(Memory.ConditionGoal, false), loop.Condition));
                    instructions.Add(new JumpFactInstruction(Memory.ConditionGoal, "==", 0, label_end));
                    instructions.AddRange(CompileBlock(method, loop.Block, label_end, label_repeat));
                    instructions.AddRange(CompileBlock(method, loop.Postfix, null, null));
                    instructions.Add(label_end);
                }
                else if (statement is ReturnStatement ret)
                {
                    if (ret.Expression is not null)
                    {
                        var result = new Address(Memory.CallResultBase, false);
                        instructions.AddRange(CompileExpression(method, result, ret.Expression));
                    }

                    instructions.Add(new JumpIndirectInstruction(Memory.RegisterBase));
                }
                else
                {
                    throw new NotSupportedException($"Statement {statement.GetType().Name} not recognized.");
                }
            }

            return instructions;
        }

        private List<Instruction> CompileExpression(Method method, Address? result, Expression expression)
        {
            var instructions = new List<Instruction>();

            if (expression is ConstExpression ce)
            {
                if (result is null)
                {
                    return instructions;
                }

                instructions.AddRange(Utils.GetPointer(Memory, result, Memory.ExpressionGoal));
                instructions.Add(new CommandInstruction($"up-set-indirect-goal g: {Memory.ExpressionGoal} c: {ce.Value}"));

                return instructions;
            }
            else if (expression is AccessorExpression ae)
            {
                if (result is null)
                {
                    return instructions;
                }

                var address = GetAddress(ae);
                instructions.AddRange(Utils.MemCpy(Memory, address, result, ae.Type!.Size));

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
                var callee_registercount = Memory.GetRegisterCount(method);
                var called_registercount = Memory.GetRegisterCount(call.Method);
                var registers_addr = new Address(Memory.RegisterBase, false);
                var stack_addr = new Address(Memory.StackPtr, true);
                var label_return = new LabelInstruction();

                instructions.AddRange(Utils.MemCpy(Memory, registers_addr, stack_addr, callee_registercount));
                instructions.AddRange(Utils.Clear(Memory.RegisterBase, called_registercount));

                // set parameters
                instructions.Add(new CommandInstruction($"up-modify-goal {Memory.RegisterBase} c:= {label_return.Label}"));

                for (int i = 0; i < call.Method.Parameters.Count; i++)
                {
                    var par = method.Parameters[i];
                    var arg = call.Arguments[i];
                    var addr = Memory.GetAddress(par);

                    if (arg is ConstExpression argc)
                    {
                        instructions.Add(new CommandInstruction($"up-modify-goal {addr.Goal} c:= {argc.Value}"));
                    }
                    else if (arg is AccessorExpression arga)
                    {
                        if (!arga.IsVariableAccess) throw new NotSupportedException($"Method {call.Method.Name} call with parameter {par.Name} not const or var.");

                        var argaddr = Memory.GetAddress(arga.Variable);

                        if (!Script.GlobalScope.Variables.Contains(arga.Variable))
                        {
                            // local variables are now on stack, so interpret addr as offset from stackptr
                            argaddr = new(Memory.StackPtr, true, argaddr.Goal - Memory.RegisterBase);
                        }

                        if (par.IsRef)
                        {
                            instructions.AddRange(Utils.GetPointer(Memory, argaddr, addr.Goal));
                        }
                        else
                        {
                            instructions.AddRange(Utils.MemCpy(Memory, argaddr, addr, par.Type.Size));
                        }
                    }
                    else
                    {
                        throw new NotSupportedException($"Method {call.Method} call with paramter {par.Name} not const or var.");
                    }
                }

                // increment stackptr and jump
                instructions.Add(new CommandInstruction($"up-modify-goal {Memory.StackPtr} c:+ {callee_registercount}"));
                instructions.Add(new JumpInstruction(MethodLabels[call.Method]));

                // return
                instructions.Add(label_return);
                instructions.Add(new CommandInstruction($"up-modify-goal {Memory.StackPtr} c:- {callee_registercount}"));
                instructions.AddRange(Utils.MemCpy(Memory, stack_addr, registers_addr, callee_registercount));
                
                if (result is not null)
                {
                    var result_addr = new Address(Memory.CallResultBase, false);
                    instructions.AddRange(Utils.MemCpy(Memory, result_addr, result, call.Method.ReturnType.Size));
                }

                return instructions;
            }
            else
            {
                throw new NotSupportedException($"Expression {expression.GetType().Name} not recognized.");
            }
        }

        private List<Instruction> InitializeArrays(Scope scope)
        {
            // first goal of array is length

            var instructions = new List<Instruction>();

            foreach (var array in scope.Variables.Where(x => x.Type is ArrayType && !x.IsRef))
            {
                var addr = Memory.GetAddress(array);
                var length = ((ArrayType)array.Type).Length;

                Debug.Assert(!addr.IsRef);
                instructions.Add(new CommandInstruction($"up-modify-goal {addr.Goal} c:= {length}"));
            }

            return instructions;
        }

        private Address GetAddress(AccessorExpression accessor)
        {
            var addr = Memory.GetAddress(accessor.Variable);

            if (accessor.IsStructAccess)
            {
                var offset = 0;
                var type = accessor.Variable.Type;

                foreach (var field in accessor.Fields!)
                {
                    offset += ((CompoundType)type).GetOffset(field);
                    type = field.Type;
                }

                addr = new(addr.Goal, addr.IsRef, offset);
            }
            else if (accessor.IsArrayAccess)
            {
                var size = ((ArrayType)accessor.Variable.Type).ElementType.Size;

                if (accessor.Index is ConstExpression ce)
                {
                    addr = new(addr.Goal, addr.IsRef, 1 + ce.Value * size);
                }
                else if (accessor.Index is AccessorExpression ai)
                {
                    if (!ai.IsVariableAccess) throw new NotSupportedException($"Index access to {accessor.Variable.Name} with recursive index.");

                    var vaddr = Memory.GetAddress(ai.Variable);

                    addr = new(addr.Goal, addr.IsRef, vaddr.Goal, size);
                }
                else
                {
                    throw new NotSupportedException($"Indexed access not allowed with index {accessor.Index!.GetType().Name}.");
                }
            }

            Debug.Assert(addr.Goal > 0);
            Debug.Assert(addr.Offset >= 0);
            Debug.Assert(addr.IndexStride >= 0);

            return addr;
        }
    }
}
