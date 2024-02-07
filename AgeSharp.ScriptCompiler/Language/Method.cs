using AgeSharp.ScriptCompiler.Language.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgeSharp.ScriptCompiler.Language
{
    public class Method
    {
        public string Name { get; }
        public Type ReturnType { get; }
        public bool ReturnsVoid => ReturnType == null;
        public IReadOnlyList<Variable> Parameters { get; }
        public Block Block { get; }

        public Method(Script script, string name, Type return_type, IReadOnlyList<Variable> parameters)
        {
            Name = name;
            ReturnType = return_type;
            Parameters = parameters.ToList();
            Block = new Block(script, null);
            Block.Scope.Variables.AddRange(Parameters);
        }
    }
}
