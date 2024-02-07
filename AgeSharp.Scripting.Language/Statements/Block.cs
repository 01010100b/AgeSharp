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

        private readonly Script Script;

        internal Block(Script script, Block? parent)
        {
            Script = script;
            Parent = parent;
            Scope = parent is null ? new(script.GlobalScope) : new(parent.Scope);
        }

        public Block CreateChild() => new(Script, this);

        public override IEnumerable<Block> GetBlocks()
        {
            yield return this;
        }

        public IEnumerable<Block> GetChildBlocks()
        {
            foreach (var statement in Statements)
            {
                foreach (var block in statement.GetBlocks())
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
