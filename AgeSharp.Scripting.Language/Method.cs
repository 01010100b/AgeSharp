using AgeSharp.Scripting.Language.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language
{
    public class Method : Validated
    {
        public string Name { get; }
        public Type? ReturnType { get; }
        public IReadOnlyList<Variable> Parameters { get; } = new List<Variable>();
        public Block Block { get; }
        public Scope Scope => Block.Scope;

        internal Method(Script script, string name, Type? return_type) : base()
        {
            Name = name;
            ReturnType = return_type;
            Block = new(script, null);
        }

        public Variable CreateParameter(string name, Type type, bool is_ref)
        {
            var variable = Scope.CreateVariable(name, type, is_ref);
            ((List<Variable>)Parameters).Add(variable);

            return variable;
        }

        public IEnumerable<Block> GetAllBlocks()
        {
            var stack = new Stack<Block>();
            stack.Push(Block);

            while (stack.Count > 0)
            {
                var block = stack.Pop();

                foreach (var child in block.GetChildBlocks())
                {
                    stack.Push(child);
                }

                yield return block;
            }
        }

        public override void Validate()
        {
            ValidateName(Name);
            ReturnType?.Validate();

            foreach (var parameter in Parameters)
            {
                parameter.Validate();
            }

            foreach (var block in GetAllBlocks())
            {
                block.Validate();
            }
        }
    }
}
