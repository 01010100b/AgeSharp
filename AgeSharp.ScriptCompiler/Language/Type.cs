using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgeSharp.ScriptCompiler.Language
{
    public class Type
    {
        public string Name { get; }
        public int Size { get; }

        public Type(string name, int size)
        {
            Name = name;
            Size = size;
        }
    }
}
