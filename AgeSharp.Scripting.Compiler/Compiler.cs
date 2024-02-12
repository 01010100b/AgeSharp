﻿using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Compiler.Rules;
using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler
{
    public class Compiler
    {
        public Script CreateScript()
        {
            var script = new Script();
            var args = new object[] { script };

            foreach (var type in GetType().Assembly.GetTypes().Where(x => x.IsAssignableTo(typeof(Intrinsic)) && !x.IsAbstract))
            {
                var intrinsic = (Intrinsic)Activator.CreateInstance(type, args)!;
                script.AddMethod(intrinsic);
            }

            return script;
        }

        public CompilationResult Compile(Script script, Settings settings)
        {
            script.Validate();
            settings.Validate();
            Transform(script, settings);

            var memory = new Memory(script, settings);

            var instruction_compiler = new InstructionCompiler(script, memory, settings);
            var instructions = instruction_compiler.Compile();
            instructions.ForEach(x => x.Validate());

            var rule_compiler = new RuleCompiler(instructions, settings);
            var rules = rule_compiler.Compile();
            rules.ForEach(x => x.Validate());

            var result = new CompilationResult(script, settings, memory, instructions, rules);

            return result;
        }

        private void Transform(Script script, Settings settings)
        {
        }
    }
}
