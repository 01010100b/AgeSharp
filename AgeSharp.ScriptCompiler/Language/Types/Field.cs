using System;
using System.Collections.Generic;
using System.Text;

namespace AgeSharp.ScriptCompiler.Language.Types
{
    public class Field
    {
        public string Name { get; }
        public Type Type { get; }

        public Field(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}
