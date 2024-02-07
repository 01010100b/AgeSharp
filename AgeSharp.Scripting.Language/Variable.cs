using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language
{
    public class Variable : Validated
    {
        public Scope Scope { get; }
        public string Name { get; }
        public Type Type { get; }
        public bool IsRef { get; }
        public int Size => IsRef ? 1 : Type.Size;

        internal Variable(Scope scope, string name, Type type, bool is_ref) : base()
        {
            Scope = scope;
            Name = name;
            Type = type;
            IsRef = is_ref;
        }

        public override void Validate()
        {
            ValidateName(Name);
            Type.Validate();
        }
    }
}
