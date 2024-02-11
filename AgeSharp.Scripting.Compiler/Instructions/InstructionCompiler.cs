using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
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

        public List<Instruction> Compile()
        {
            throw new NotImplementedException();
        }

        private List<Instruction> CompileMethod(Method method)
        {
            throw new NotImplementedException();
        }
    }
}
