using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Types
{
    public class Field(string name, Type type) : Validated
    {
        public string Name { get; } = name;
        public Type Type { get; } = type;

        public override void Validate()
        {
            ValidateName(Name);
            Type.Validate();
        }
    }
}
