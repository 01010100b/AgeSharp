﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Expressions
{
    public class CallExpression(Method method, string? literal = null) : Expression
    {
        public Method Method { get; } = method;
        public override Type? Type => Method.ReturnType;
        public string? Literal { get; } = literal;
        public List<Expression> Arguments { get; } = [];

        public override void Validate()
        {
            throw new NotImplementedException();
        }
    }
}
