using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Instructions
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
                instructions.Add(new CommandInstruction($"up-modify-goal {memory.AddressingGoal} g:= {address.Offset}"));
                instructions.Add(new CommandInstruction($"up-modify-goal {memory.AddressingGoal} c:* {address.IndexStride}"));
                instructions.Add(new CommandInstruction($"up-modify-goal {memory.AddressingGoal} c:+ 1"));
                instructions.Add(new CommandInstruction($"up-modify-goal {goal} g:+ {memory.AddressingGoal}"));
            }
            else if (address.Offset > 0)
            {
                instructions.Add(new CommandInstruction($"up-modify-goal {goal} c:+ {address.Offset}"));
            }

            return instructions;
        }

        public static List<Instruction> MemCpy(Memory memory, Address from, Address to, int length)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);

            var instructions = new List<Instruction>();
            var label_return = new LabelInstruction();

            instructions.AddRange(GetPointer(memory, from, memory.MemCpy0));
            instructions.AddRange(GetPointer(memory, to, memory.MemCpy1));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.MemCpy2} c:= {length}"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.AddressingGoal} c:= {label_return.Label}"));
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
            instructions.Add(new JumpFactInstruction(memory.MemCpy2, "==", 0, label_end));
            instructions.Add(new CommandInstruction($"up-get-indirect-goal g: {memory.MemCpy0} {memory.MemCpy3}"));
            instructions.Add(new CommandInstruction($"up-set-indirect-goal g: {memory.MemCpy1} g: {memory.MemCpy3}"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.MemCpy0} c:+ 1"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.MemCpy1} c:+ 1"));
            instructions.Add(new CommandInstruction($"up-modify-goal {memory.MemCpy2} c:- 1"));
            instructions.Add(new JumpInstruction(label_repeat));

            instructions.Add(label_end);
            instructions.Add(new JumpIndirectInstruction(memory.AddressingGoal));

            return instructions;
        }
    }
}
