using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Types
{
    public class CompoundType : Type
    {
        public override int Size => Fields.Sum(x => x.Type.Size);
        public List<Field> Fields { get; } = [];

        internal CompoundType(string name) : base(name) { }

        public override void Validate()
        {
            base.Validate();

            if (Fields.Count < 1) throw new Exception($"CompoundType {Name} has no fields.");
        }
    }
}
