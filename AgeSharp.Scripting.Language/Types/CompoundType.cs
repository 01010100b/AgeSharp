using AgeSharp.Common;
using System.Text;

namespace AgeSharp.Scripting.Language.Types
{
    public class CompoundType(string name) : Type(name)
    {
        public override int Size => Fields.Sum(x => x.Type.Size);
        public IReadOnlyList<Field> Fields { get; } = new List<Field>();

        public void AddField(Field field)
        {
            if (Fields.Select(x => x.Name).Contains(field.Name)) throw new Exception($"Type {Name} already has field {field.Name}.");

            ((List<Field>)Fields).Add(field);
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
            ValidateName(Name);
            Throw.If<NotSupportedException>(!Fields.Any(), $"CompoundType {Name} has no fields.");

            foreach (var field in Fields)
            {
                field.Validate();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{Name} (size {Size})");
            sb.AppendLine("{");

            foreach (var field in Fields)
            {
                sb.AppendLine($"\t{field};");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
