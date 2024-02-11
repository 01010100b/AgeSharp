using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Statements
{
    public class Block : Statement
    {
        public Block? Parent { get; }
        public Scope Scope { get; }
        public List<Statement> Statements { get; } = [];

        public Block(Block parent)
        {
            Parent = parent;
            Scope = new(parent.Scope);
        }

        public Block(Script script)
        {
            Parent = null;
            Scope = new(script.GlobalScope);
        }

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

            foreach (var statement in Statements)
            {
                statement.Validate();
            }
        }
    }
}
