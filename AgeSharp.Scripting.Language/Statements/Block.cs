using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Statements
{
    public class Block(Scope scope) : Statement
    {
        public override Scope Scope { get; } = new(scope);
        public List<Statement> Statements { get; } = [];

        public override IEnumerable<Block> GetContainedBlocks()
        {
            yield return this;
        }

        public IEnumerable<Block> GetChildBlocks()
        {
            foreach (var statement in Statements)
            {
                foreach (var block in statement.GetContainedBlocks())
                {
                    yield return block;
                }
            }
        }

        public override void Validate()
        {
            Scope.Validate();

            for (int i = 0; i < Statements.Count; i++)
            {
                var statement = Statements[i];
                statement.Validate();

                if (statement is ReturnStatement && i != Statements.Count - 1) throw new NotSupportedException($"Return statement not last statement of block.");
            }
        }
    }
}
