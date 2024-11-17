using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language.Types;
using System.Diagnostics;

namespace AgeSharp.Scripting.Compiler
{
    internal static class Utils
    {
        private static LabelInstruction MemCpyLabel { get; } = new();

        public static List<Instruction> Clear(int from, int length, int value = 0)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

            var instructions = new List<Instruction>();

            while (length > 0)
            {
                if (length >= 4 && from >= 41 && from < 508 && value == 0)
                {
                    instructions.Add(new CommandInstruction($"up-setup-cost-data 1 {from}"));
                    from += 4;
                    length -= 4;
                }
                else
                {
                    instructions.Add(new CommandInstruction($"up-modify-goal {from} c:= {value}"));
                    from++;
                    length--;
                }

            }

            return instructions;
        }

        public static Address GetAddress(Memory memory, AccessorExpression accessor)
        {
            var addr = memory.GetAddress(accessor.Variable);
            Debug.Assert(!addr.IsRef);

            if (accessor.IsStructAccess)
            {
                var offset = 0;
                var type = accessor.Variable.Type.ProperType;

                foreach (var field in accessor.Fields!)
                {
                    offset += ((CompoundType)type).GetOffset(field);
                    type = field.Type;
                }

                addr = new(type, addr.Goal, accessor.Variable.Type is RefType, offset);
            }
            else if (accessor.IsArrayAccess)
            {
                var type = ((ArrayType)accessor.Variable.Type.ProperType).ElementType;
                var size = type.Size;

                if (accessor.Index is ConstExpression ce)
                {
                    if (ce.Value == -1)
                    {
                        addr = new(PrimitiveType.Int, addr.Goal, accessor.Variable.Type is RefType);
                    }
                    else
                    {
                        addr = new(type, addr.Goal, accessor.Variable.Type is RefType, 1 + ce.Value * size);
                    }
                }
                else if (accessor.Index is AccessorExpression ai)
                {
                    if (!ai.IsVariableAccess) throw new NotSupportedException($"Index access to {accessor.Variable.Name} with recursive index.");

                    var vaddr = memory.GetAddress(ai.Variable);
                    Debug.Assert(vaddr.IsDirect);

                    addr = new(type, addr.Goal, accessor.Variable.Type is RefType, vaddr.DirectGoal, size);
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

        public static List<Instruction> Assign(Memory memory, int from_goal, Address to)
        {
            var from_type = to.Type is RefType rt ? rt.ReferencedType : to.Type;
            var from = new Address(from_type, from_goal, false);

            return Assign(memory, from, to);
        }

        public static List<Instruction> Assign(Memory memory, Address from, Address to, bool ref_assign = false)
        {
            // uses Utils5-Utils6

            to.Type.ValidateAssignmentFrom(from.Type);

            var instructions = new List<Instruction>();

            if (from.Type is RefType frt)
            {
                if (to.Type is RefType trt)
                {
                    ref_assign |= frt.ReferencedType is ArrayType || trt.ReferencedType is ArrayType;

                    if (ref_assign)
                    {
                        // copy pointer
                        instructions.AddRange(MemCpy(memory, from, to, 1));
                    }
                    else
                    {
                        // copy value
                        instructions.AddRange(GetPointer(memory, from, memory.Utils5));
                        instructions.Add(new CommandInstruction($"up-get-indirect-goal g: {memory.Utils5} {memory.Utils5}"));
                        instructions.AddRange(GetPointer(memory, to, memory.Utils6));
                        instructions.Add(new CommandInstruction($"up-get-indirect-goal g: {memory.Utils6} {memory.Utils6}"));
                        var fromaddr = new Address(frt.ReferencedType, memory.Utils5, true);
                        var toaddr = new Address(trt.ReferencedType, memory.Utils6, true);
                        instructions.AddRange(MemCpy(memory, fromaddr, toaddr, toaddr.Type.Size));
                    }
                }
                else
                {
                    // dereference
                    instructions.AddRange(GetPointer(memory, from, memory.Utils5));
                    instructions.Add(new CommandInstruction($"up-get-indirect-goal g: {memory.Utils5} {memory.Utils5}"));
                    var ptraddr = new Address(PrimitiveType.Void, memory.Utils5, true);
                    instructions.AddRange(MemCpy(memory, ptraddr, to, to.Type.Size));
                }
            }
            else
            {
                if (to.Type is RefType trt)
                {
                    ref_assign |= trt.ReferencedType is ArrayType;

                    if (ref_assign)
                    {
                        // take address of
                        instructions.AddRange(GetPointer(memory, from, to));
                    }
                    else
                    {
                        // copy value
                        instructions.AddRange(GetPointer(memory, to, memory.Utils5));
                        instructions.Add(new CommandInstruction($"up-get-indirect-goal g: {memory.Utils5} {memory.Utils5}"));
                        var ptraddr = new Address(PrimitiveType.Void, memory.Utils5, true);
                        instructions.AddRange(MemCpy(memory, from, ptraddr, trt.ReferencedType.Size));
                    }

                }
                else
                {
                    // copy value
                    instructions.AddRange(MemCpy(memory, from, to, to.Type.Size));
                }
            }

            return instructions;
        }

        public static List<Instruction> GetPointer(Memory memory, Address address, int goal)
        {
            // uses Utils4

            var instructions = new List<Instruction>();

            if (address.IsRef)
            {
                instructions.Add(new CommandInstruction($"up-modify-goal {goal} g:= {address.Goal}"));
            }
            else
            {
                instructions.Add(new CommandInstruction($"up-modify-goal {goal} c:= {address.Goal}"));
            }

            if (address.IsArrayAccess)
            {
                instructions.Add(new CommandInstruction($"up-modify-goal {memory.Utils4} g:= {address.Offset}"));
                instructions.Add(new CommandInstruction($"up-modify-goal {memory.Utils4} c:* {address.IndexStride}"));
                instructions.Add(new CommandInstruction($"up-modify-goal {memory.Utils4} c:+ 1"));
                instructions.Add(new CommandInstruction($"up-modify-goal {goal} g:+ {memory.Utils4}"));
            }
            else if (address.Offset > 0)
            {
                instructions.Add(new CommandInstruction($"up-modify-goal {goal} c:+ {address.Offset}"));
            }

            return instructions;
        }

        public static List<Instruction> GetPointer(Memory memory, Address address, Address pointer)
        {
            var instructions = new List<Instruction>();

            instructions.AddRange(GetPointer(memory, address, memory.Utils0));
            instructions.AddRange(GetPointer(memory, pointer, memory.Utils1));
            instructions.Add(new CommandInstruction($"up-set-indirect-goal g: {memory.Utils1} g: {memory.Utils0}"));

            return instructions;
        }

        public static List<Instruction> MemCpy(Memory memory, Address from, Address to, int length)
        {
            // ignores the types of the adresses
            // uses Utils0-Utils4

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

            var instructions = new List<Instruction>();
            var label_return = new LabelInstruction();

            if (from.IsDirect && to.IsDirect)
            {
                if (length < 10)
                {
                    var from_goal = from.DirectGoal;
                    var to_goal = to.DirectGoal;

                    for (int i = 0; i < length; i++)
                    {
                        instructions.Add(new CommandInstruction($"up-modify-goal {to_goal} g:= {from_goal}"));
                        from_goal++;
                        to_goal++;
                    }

                    return instructions;
                }
            }

            instructions.AddRange(GetPointer(memory, from, memory.Utils0));
            instructions.AddRange(GetPointer(memory, to, memory.Utils1));

            if (length < 5)
            {
                for (int i = 0; i < length; i++)
                {
                    instructions.Add(new CommandInstruction($"up-get-indirect-goal g: {memory.Utils0} {memory.Utils3}"));
                    instructions.Add(new CommandInstruction($"up-set-indirect-goal g: {memory.Utils1} g: {memory.Utils3}"));

                    if (i < length - 1)
                    {
                        instructions.Add(new CommandInstruction($"up-modify-goal {memory.Utils0} c:+ 1"));
                        instructions.Add(new CommandInstruction($"up-modify-goal {memory.Utils1} c:+ 1"));
                    }
                }

                return instructions;
            }

            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Utils2} c:= {length}"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Utils4} c:= {label_return.Label}"));
            instructions.Add(new JumpInstruction(MemCpyLabel));
            instructions.Add(label_return);

            return instructions;
        }

        public static List<Instruction> GetBinarySearch(Memory memory, string fact, int goal, int min = 0, int max = 1000)
        {
            var instructions = new List<Instruction>();
            var label_repeat = new LabelInstruction();
            var label_end = new LabelInstruction();
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Utils0} c:= {min}"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Utils1} c:= {max}"));

            instructions.Add(label_repeat);
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Utils2} g:= {memory.Utils1}"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Utils2} g:- {memory.Utils0}"));
            instructions.Add(new JumpFactInstruction($"up-compare-goal {memory.Utils2} c:<= 1", label_end));

            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Utils2} g:= {memory.Utils0}"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Utils2} g:+ {memory.Utils1}"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Utils2} c:z/ 2"));
            instructions.Add(new RuleInstruction($"{fact} g:>= {memory.Utils2}", $"up-modify-goal {memory.Utils0} g:= {memory.Utils2}"));
            instructions.Add(new RuleInstruction($"{fact} g:< {memory.Utils2}", $"up-modify-goal {memory.Utils1} g:= {memory.Utils2}"));
            instructions.Add(new JumpInstruction(label_repeat));

            instructions.Add(label_end);
            instructions.Add(new CommandInstruction($"up-modify-goal {goal} g:= {memory.Utils0}"));

            return instructions;
        }

        public static List<Instruction> CompileUtils(Memory memory)
        {
            // memcpy

            var instructions = new List<Instruction>() { MemCpyLabel };
            var label_repeat = new LabelInstruction();
            var label_end = new LabelInstruction();

            instructions.Add(label_repeat);
            instructions.Add(new JumpFactInstruction(memory.Utils2, "==", 0, label_end));
            instructions.Add(new CommandInstruction($"up-get-indirect-goal g: {memory.Utils0} {memory.Utils3}"));
            instructions.Add(new CommandInstruction($"up-set-indirect-goal g: {memory.Utils1} g: {memory.Utils3}"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Utils0} c:+ 1"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Utils1} c:+ 1"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Utils2} c:- 1"));
            instructions.Add(new JumpInstruction(label_repeat));

            instructions.Add(label_end);
            instructions.Add(new JumpIndirectInstruction(memory.Utils4));

            return instructions;
        }
    }
}
