using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Statements
{
    public class LoopStatement : Statement
    {
        public override Scope Scope => ScopingBlock.Scope;
        public Expression Condition { get; }
        public Block ScopingBlock { get; } // variables defined in Prefix go here instead
        public Block Prefix { get; }
        public Block Block { get; }
        public Block Postfix { get; }

        public LoopStatement(Scope scope, Expression condition) : base()
        {
            Condition = condition;
            ScopingBlock = new(scope); 
            Prefix = new(ScopingBlock.Scope);
            Block = new(ScopingBlock.Scope);
            Postfix = new(ScopingBlock.Scope);
        }

        public override IEnumerable<Block> GetContainedBlocks()
        {
            yield return ScopingBlock;
            yield return Prefix;
            yield return Block;
            yield return Postfix;
        }

        public override void Validate()
        {
            ValidateExpression(Condition);
            if (ScopingBlock.Statements.Count > 0) throw new NotSupportedException($"LoopStatement ScopingBlock has statements.");
            if (Prefix.Scope.Variables.Count > 0) throw new NotSupportedException($"LoopStatement Prefix has variables.");
        }
    }
}
