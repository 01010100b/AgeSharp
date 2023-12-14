using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AgeSharp.ScriptCompiler.Language.Types
{
    public class CompoundType : Type
    {
        public IReadOnlyList<Field> Fields { get; }

        public CompoundType(string name, IReadOnlyList<Field> fields) : base(name, fields.Sum(x => x.Type.Size))
        {
            Fields = fields;
        }

        internal int GetOffset(Field field)
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

            throw new ArgumentOutOfRangeException(nameof(field));
        }
    }
}
