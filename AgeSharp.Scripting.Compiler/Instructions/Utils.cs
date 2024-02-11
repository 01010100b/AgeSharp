using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal static class Utils
    {
        private static string MemCpyLabel { get; } = Guid.NewGuid().ToString();

        public static List<Instruction> Clear(int from, int to, int value = 0)
        {
            var instructions = new List<Instruction>();

            var length = to - from;

            if (length <= 0)
            {
                return instructions;
            }

            while (length > 0)
            {
                instructions.Add(new CommandInstruction($"up-modify-goal {from} c:= {value}"));
                from++;
                length--;
            }

            return instructions;
        }

        public static List<Instruction> GetPointer(Address address, int goal)
        {
            throw new NotImplementedException();
        }

        public static List<Instruction> MemCpy(Memory memory, Address from, Address to, int length)
        {
            throw new NotImplementedException();
        }

        public static List<Instruction> CompileMemCpy(Memory memory)
        {
            throw new NotImplementedException();
        }
    }
}
