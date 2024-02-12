using AgeSharp.Scripting.Language.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language
{
    public class Variable : Validated
    {
        public string Name { get; }
        public Type Type { get; }
        public bool IsRef { get; }
        public int Size => IsRef ? 1 : Type.Size;

        public Variable(Scope scope, string name, Type type, bool is_ref)
        {
            Name = name;
            Type = type;
            IsRef = is_ref;
            scope.AddVariable(this);
        }

        public bool CanAssign(Type type)
        {
            throw new NotImplementedException();
        }

        public override void Validate()
        {
            ValidateName(Name);

            if (Type == PrimitiveType.Void) throw new NotSupportedException($"Variable {Name} has type Void.");
        }
    }
}
