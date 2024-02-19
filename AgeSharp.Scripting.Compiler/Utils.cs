using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Language.Types;

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
                instructions.Add(new CommandInstruction($"up-modify-goal {from} c:= {value}"));
                from++;
                length--;
            }

            return instructions;
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
                    instructions.AddRange(GetPointer(memory, from, memory.ExpressionGoal));
                    instructions.Add(new CommandInstruction($"up-get-indirect-goal g: {memory.ExpressionGoal} {memory.ExpressionGoal}"));
                    var ptraddr = new Address(PrimitiveType.Void, memory.ExpressionGoal, true);
                    instructions.AddRange(MemCpy(memory, ptraddr, to, to.Type.Size));
                }
            }
            else
            {
                if (to.Type is RefType)
                {
                    instructions.AddRange(GetPointer(memory, from, to));
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

        public static List<Instruction> MemCpy(Memory memory, Address from, Address to, int length)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

            var instructions = new List<Instruction>();
            var label_return = new LabelInstruction();

            instructions.AddRange(GetPointer(memory, from, memory.Sp0));
            instructions.AddRange(GetPointer(memory, to, memory.Sp1));
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
