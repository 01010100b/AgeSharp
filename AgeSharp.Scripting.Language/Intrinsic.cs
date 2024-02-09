using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language
{
    public abstract class Intrinsic(Script script, string name, Type? return_type, bool has_string_literal) : Method(script, name, return_type)
    {
        public bool HasStringLiteral { get; } = has_string_literal;
        public List<Variable> ConstParameters { get; } = [];
    }
}
