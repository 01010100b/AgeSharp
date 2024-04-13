﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Statements
{
    public class SkipStatement(Scope scope, Block block) : Statement
    {
        public override Scope Scope { get; } = scope;
        public Block Block { get; } = block;

        public override IEnumerable<Block> GetContainedBlocks() => Enumerable.Empty<Block>();

        public override IEnumerable<Expression> GetContainedExpressions() => Enumerable.Empty<Expression>();

        public override void Validate()
        {

        }

        public override string ToString()
        {
            return "skip;";
        }
    }
}
