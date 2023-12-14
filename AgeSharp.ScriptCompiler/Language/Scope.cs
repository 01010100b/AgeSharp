using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace AgeSharp.ScriptCompiler.Language
{
    public class Scope
    {
        public Scope Parent { get; }
        public List<Variable> Variables { get; } = new List<Variable>();
        public int Size => Variables.Sum(x => x.Size);

        public Scope(Scope parent)
        {
            Parent = parent;
        }

        public Variable GetVariable(string name)
        {
            var current = this;

            while (current != null)
            {
                foreach (var v in current.Variables)
                {
                    if (v.Name == name)
                    {
                        return v;
                    }
                }

                current = current.Parent;
            }

            throw new Exception($"Variable {name} not found.");
        }
    }
}
