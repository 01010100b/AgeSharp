using System;
using System.Collections.Generic;
using System.Text;

namespace AgeSharp.ScriptCompiler.Language
{
    public abstract class Expression
    {
        public abstract Type Type { get; }
    }
}
