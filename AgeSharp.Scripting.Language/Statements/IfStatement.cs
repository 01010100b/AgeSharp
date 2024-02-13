﻿using AgeSharp.Scripting.Language.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Statements
{
    public class IfStatement : Statement
    {
        public override Scope Scope { get; }
        public Expression Condition { get; }
        public Block WhenTrue { get; }
        public Block WhenFalse { get; }

        public IfStatement(Scope scope, Expression condition) : base()
        {
            Scope = scope;
            Condition = condition;
            WhenTrue = new(scope);
            WhenFalse = new(scope);
        }

        public override IEnumerable<Block> GetContainedBlocks()
        {
            yield return WhenTrue;
            yield return WhenFalse;
        }

        public override void Validate()
        {
            if (Condition.Type is null || Condition.Type != PrimitiveType.Bool) throw new NotSupportedException($"If condition does not have type Bool.");
            
            ValidateExpression(Condition);
        }
    }
}