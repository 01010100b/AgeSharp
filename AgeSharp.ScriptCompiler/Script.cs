using AgeSharp.ScriptCompiler.Compiler;
using AgeSharp.ScriptCompiler.Language;
using AgeSharp.ScriptCompiler.Language.Types;
using System.Collections.Generic;
using System.Linq;

namespace AgeSharp.ScriptCompiler
{
    public class Script
    {
        public Type Int => Types.Single(x => x.Name == "Int");
        public Type Bool => Types.Single(x => x.Name == "Bool");

        public List<Type> Types { get; } = new List<Type>();
        public List<Method> Methods { get; } = new List<Method>();
        public Scope GlobalScope { get; } = new Scope(null);

        public Script()
        {
            GenerateBuiltinTypes();
        }

        public CompilationResult Compile()
        {
            PreCompiler.Compile(this);
            var memory = MemoryCompiler.Compile(this, 512);
            var instructions = InstructionCompiler.Compile(this, memory);
            var rules = RuleCompiler.Compile(instructions, 16);
            var per = PerCompiler.Compile(rules);

            return new CompilationResult(instructions, rules, per, memory);
        }

        private void GenerateBuiltinTypes()
        {
            var int_type = new Type("Int", 1);
            var bool_type = new Type("Bool", 1);
            var point_type = new CompoundType("Point", new List<Field>()
            {
                new Field("X", int_type),
                new Field("Y", int_type)
            });
            var cost_type = new CompoundType("Cost", new List<Field>()
            {
                new Field("Food", int_type),
                new Field("Wood", int_type),
                new Field("Stone", int_type),
                new Field("Gold", int_type)
            });

            Types.Add(int_type);
            Types.Add(bool_type);
            Types.Add(point_type);
            Types.Add(cost_type);
        }
    }
}
