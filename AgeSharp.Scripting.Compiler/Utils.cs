using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language.Types;
using AgeSharp.Scripting.Language;
using Type = AgeSharp.Scripting.Language.Type;
using AgeSharp.Scripting.Language.Expressions;
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
                if (length >= 4 && from >= 41 && from < 508)
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
                var type = accessor.Variable.Type;

                foreach (var field in accessor.Fields!)
                {
                    offset += ((CompoundType)type).GetOffset(field);
                    type = field.Type;
                }

                addr = new(type, addr.Goal, false, offset);
            }
            else if (accessor.IsArrayAccess)
            {
                var type = ((ArrayType)accessor.Variable.Type).ElementType;
                var size = type.Size;

                if (accessor.Index is ConstExpression ce)
                {
                    addr = new(type, addr.Goal, false, 1 + ce.Value * size);
                }
                else if (accessor.Index is AccessorExpression ai)
                {
                    if (!ai.IsVariableAccess) throw new NotSupportedException($"Index access to {accessor.Variable.Name} with recursive index.");

                    var vaddr = memory.GetAddress(ai.Variable);

                    addr = new(type, addr.Goal, false, vaddr.Goal, size);
                }
                else
                {
                    throw new NotSupportedException($"Indexed access not allowed with index {accessor.Index!.GetType().Name}.");
                }
            }

            Debug.Assert(addr.Goal > 0);
            Debug.Assert(!addr.IsRef);
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

        public static List<Instruction> Assign(Memory memory, Address from, Address to)
        {
            to.Type.ValidateAssignmentFrom(from.Type);

            var instructions = new List<Instruction>();

            if (from.Type is RefType)
            {
                if (to.Type is RefType)
                {
                    instructions.AddRange(MemCpy(memory, from, to, 1));
                }
                else
                {
                    instructions.AddRange(GetPointer(memory, from, memory.Sp5));
                    instructions.Add(new CommandInstruction($"up-get-indirect-goal g: {memory.Sp5} {memory.Sp5}"));
                    var ptraddr = new Address(PrimitiveType.Void, memory.Sp5, true);
                    instructions.AddRange(MemCpy(memory, ptraddr, to, to.Type.Size));
                }
            }
            else
            {
                if (to.Type is RefType rt)
                {
                    instructions.AddRange(GetPointer(memory, to, memory.Sp5));
                    instructions.Add(new CommandInstruction($"up-get-indirect-goal g: {memory.Sp5} {memory.Sp5}"));
                    var ptraddr = new Address(PrimitiveType.Void, memory.Sp5, true);
                    instructions.AddRange(MemCpy(memory, from, ptraddr, rt.ReferencedType.Size));
                }
                else
                {
                    instructions.AddRange(MemCpy(memory, from, to, to.Type.Size));
                }
            }

            return instructions;
        }

        public static List<Instruction> GetPointer(Memory memory, Address address, int goal)
        {
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
                instructions.Add(new CommandInstruction($"up-modify-goal {memory.Sp4} g:= {address.Offset}"));
                instructions.Add(new CommandInstruction($"up-modify-goal {memory.Sp4} c:* {address.IndexStride}"));
                instructions.Add(new CommandInstruction($"up-modify-goal {memory.Sp4} c:+ 1"));
                instructions.Add(new CommandInstruction($"up-modify-goal {goal} g:+ {memory.Sp4}"));
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

            instructions.AddRange(GetPointer(memory, address, memory.Sp0));
            instructions.AddRange(GetPointer(memory, pointer, memory.Sp1));
            instructions.Add(new CommandInstruction($"up-set-indirect-goal g: {memory.Sp1} g: {memory.Sp0}"));

            return instructions;
        }

        public static List<Instruction> MemCpy(Memory memory, int from_goal, Address to, int length)
        {
            var from = new Address(PrimitiveType.Void, from_goal, false);

            return MemCpy(memory, from, to, length);
        }

        // ignores the types of the adresses
        public static List<Instruction> MemCpy(Memory memory, Address from, Address to, int length)
        {
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

            instructions.AddRange(GetPointer(memory, from, memory.Sp0));
            instructions.AddRange(GetPointer(memory, to, memory.Sp1));
            
            if (length < 5)
            {
                for (int i = 0; i < length; i++)
                {
                    instructions.Add(new CommandInstruction($"up-get-indirect-goal g: {memory.Sp0} {memory.Sp3}"));
                    instructions.Add(new CommandInstruction($"up-set-indirect-goal g: {memory.Sp1} g: {memory.Sp3}"));

                    if (i < length - 1)
                    {
                        instructions.Add(new CommandInstruction($"up-modify-goal {memory.Sp0} c:+ 1"));
                        instructions.Add(new CommandInstruction($"up-modify-goal {memory.Sp1} c:+ 1"));
                    }
                }

                return instructions;
            }

            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Sp2} c:= {length}"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Sp4} c:= {label_return.Label}"));
            instructions.Add(new JumpInstruction(MemCpyLabel));
            instructions.Add(label_return);

            return instructions;
        }

        public static List<Instruction> CompileMemCpy(Memory memory)
        {
            var instructions = new List<Instruction>() { MemCpyLabel };
            var label_repeat = new LabelInstruction();
            var label_end = new LabelInstruction();

            instructions.Add(label_repeat);
            instructions.Add(new JumpFactInstruction(memory.Sp2, "==", 0, label_end));
            instructions.Add(new CommandInstruction($"up-get-indirect-goal g: {memory.Sp0} {memory.Sp3}"));
            instructions.Add(new CommandInstruction($"up-set-indirect-goal g: {memory.Sp1} g: {memory.Sp3}"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Sp0} c:+ 1"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Sp1} c:+ 1"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.Sp2} c:- 1"));
            instructions.Add(new JumpInstruction(label_repeat));

            instructions.Add(label_end);
            instructions.Add(new JumpIndirectInstruction(memory.Sp4));

            return instructions;
        }
    }
}
