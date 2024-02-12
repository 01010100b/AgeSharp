using AgeSharp.Scripting.Language.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace AgeSharp.Scripting.Language
{
    public abstract class Statement : Validated
    {
        public abstract Scope Scope { get; }
        public abstract IEnumerable<Block> GetContainedBlocks();

        protected void ValidateExpression(Expression expression)
        {
            expression.Validate();

            foreach (var variable in expression.GetReferencedVariables())
            {
                if (!Scope.IsInScope(variable))
                {
                    throw new NotSupportedException($"Variable {variable} is not in scope.");
                }
            }
        }
    }
}
