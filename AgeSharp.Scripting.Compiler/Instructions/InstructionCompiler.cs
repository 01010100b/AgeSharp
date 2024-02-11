using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;
using AgeSharp.Scripting.Language.Statements;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            foreach (var method in Script.Methods.Where(x => x is not Intrinsic))
            {
                MethodLabels[method] = new();
            }

            var instructions = new List<Instruction>();

            instructions.AddRange(CompileMethod(Script.EntryPoint!));

            foreach (var method in Script.Methods.Where(x => x is not Intrinsic && x != Script.EntryPoint))
            {
                instructions.AddRange(CompileMethod(method));
            }

            instructions.Add(LabelEnd);

            return instructions;
        }

        private List<Instruction> CompileMethod(Method method)
        {
            var instructions = new List<Instruction>() { MethodLabels[method] };

            instructions.AddRange(CompileBlock(method.Block, null, null));

            throw new NotImplementedException();
        }

        private List<Instruction> CompileBlock(Block block, LabelInstruction? label_break, LabelInstruction? label_continue)
        {
            throw new NotImplementedException();
        }

        private List<Instruction> CompileExpression(Address? result, Expression expression)
        {
            throw new NotImplementedException();
        }

        private Address GetAddress(AccessorExpression accessor)
        {
            if (accessor.Index is null && accessor.Fields is null)
            {
                return Memory.GetAddress(accessor.Variable);
            }

            throw new NotImplementedException();
        }
    }
}
