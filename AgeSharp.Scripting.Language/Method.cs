using AgeSharp.Scripting.Language.Statements;
using AgeSharp.Scripting.Language.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language
{
    public class Method : Validated
    {
        public string Name { get; set; }
        public Type ReturnType { get; set; } = PrimitiveType.Void;
        public IReadOnlyList<Variable> Parameters { get; } = new List<Variable>();
        public Block Block { get; }

        public Method(Script script) : base()
        {
            Name = GetType().Name;
            Block = new(script.GlobalScope);
        }

        public void AddParameter(Variable parameter)
        {
            ((List<Variable>)Parameters).Add(parameter);
            Block.Scope.AddVariable(parameter);
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
            ReturnType.Validate();

            foreach (var parameter in Parameters)
            {
                parameter.Validate();
            }

            if (Block.Statements.Count == 0) throw new NotSupportedException($"Method {Name} has no statements.");
            if (Block.Statements[^1] is not ReturnStatement) throw new NotSupportedException($"Last statement of method {Name} is not return statement.");

            foreach (var block in GetAllBlocks())
            {
                block.Validate();

                foreach (var variable in block.Scope.Variables.Where(x => !Parameters.Contains(x)))
                {
                    if (variable.IsRef)
                    {
                        throw new Exception($"Variable {variable.Name} is ref but not method parameter.");
                    }
                }
            }
        }
    }
}
