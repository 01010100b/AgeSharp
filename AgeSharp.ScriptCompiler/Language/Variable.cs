using System;
using System.Collections.Generic;
using System.Text;

namespace AgeSharp.ScriptCompiler.Language
{
    public class Variable
    {
        public string Name { get; }
        public Type Type { get; }
        public bool IsRef { get; }
        public int Size => IsRef ? 1 : Type.Size;

        public Variable(string name, Type type, bool is_ref = false)
        {
            Name = name;
            Type = type;
            IsRef = is_ref;
        }
    }
}
