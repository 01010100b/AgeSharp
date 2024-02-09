using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal static class Utils
    {
        public static List<Instruction> Clear(int from, int to, int value = 0)
        {
            var instructions = new List<Instruction>();

            for (int i = from; i <= to; i++)
            {
                instructions.Add(new CommandInstruction($"up-modify-goal {i} c:= {value}"));
            }

            return instructions;
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
