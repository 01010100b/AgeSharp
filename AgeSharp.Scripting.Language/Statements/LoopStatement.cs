using AgeSharp.Common;
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
        public bool ConditionAtTop { get; }
        public Block ScopingBlock { get; } // variables defined in other blocks go here instead
        public Block Before { get; }
        public Block Body { get; }
        public Block AtLoopBottom { get; }

        public LoopStatement(Block scoping_block, Expression condition, Block before, Block body, Block at_loop_bottom, bool condition_at_top = true) : base()
        {
            Throw.If<NotImplementedException>(!condition_at_top, "Condition at bottom not yet implemented.");
            Condition = condition;
            ConditionAtTop = condition_at_top;
            ScopingBlock = scoping_block; 
            Before = before;
            Body = body;
            AtLoopBottom = at_loop_bottom;
        }

        public override IEnumerable<Block> GetContainedBlocks()
        {
            yield return ScopingBlock;
            yield return Before;
            yield return Body;
            yield return AtLoopBottom;
        }

        public override IEnumerable<Expression> GetContainedExpressions()
        {
            yield return Condition;
        }

        public override void Validate()
        {
            ValidateExpression(Condition);
            if (ScopingBlock.Statements.Count > 0) throw new NotSupportedException($"LoopStatement ScopingBlock has statements.");
            if (Before.Scope.Variables.Count > 0) throw new NotSupportedException($"LoopStatement Prefix has variables.");
            Throw.If<NotSupportedException>(Body.Scope.Variables.Any(), "LoopStatement body has variables.");
            Throw.If<NotSupportedException>(AtLoopBottom.Scope.Variables.Any(), "LoopStatement AtLoopBottom has variables.");
        }

        public override string ToString()
        {
            ScopingBlock.Statements.Add(Before);
            ScopingBlock.Statements.Add(Body);
            ScopingBlock.Statements.Add(AtLoopBottom);

            var sb = new StringBuilder();
            sb.AppendLine($"loop ({Condition})");
            sb.AppendLine(ScopingBlock.ToString());

            ScopingBlock.Statements.Clear();


            return sb.ToString();
        }
    }
}
