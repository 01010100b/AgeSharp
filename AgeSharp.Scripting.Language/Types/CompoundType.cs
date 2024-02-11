using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Types
{
    public class CompoundType(string name) : Type(name)
    {
        public override int Size => Fields.Sum(x => x.Type.Size);
        public IEnumerable<Field> Fields { get; } = new List<Field>();

        public void AddField(Field field)
        {
            if (Fields.Select(x => x.Name).Contains(field.Name)) throw new Exception($"Type {Name} already has field {field.Name}.");

            ((List<Field>) Fields).Add(field);
        }

        public int GetOffset(Field field)
        {
            var offset = 0;

            foreach (var f in Fields)
            {
                if (field == f)
                {
                    return offset;
                }

                offset += f.Type.Size;
            }

            throw new Exception($"Field {field.Name} not found in type {Name}");
        }

        public override void Validate()
        {
            base.Validate();

            if (!Fields.Any()) throw new Exception($"CompoundType {Name} has no fields.");
        }
    }
}
