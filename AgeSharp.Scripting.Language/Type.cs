using AgeSharp.Scripting.Language.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language
{
    public abstract class Type(string name) : Validated
    {
        public string Name { get; } = name;
        public abstract int Size { get; }

        public override void Validate()
        {
            ValidateName(Name);
            if (Size <= 0) throw new Exception($"Type {Name} has size {Size} <= 0.");
        }
    }
}
