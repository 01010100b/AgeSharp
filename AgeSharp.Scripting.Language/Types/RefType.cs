namespace AgeSharp.Scripting.Language.Types
{
    public class RefType(Type type) : Type($"ref {type.Name}")
    {
        public Type ReferencedType { get; } = type;
        public override int Size => 1;

        public override void Validate()
        {
            if (ReferencedType is RefType) throw new NotSupportedException($"Ref type {Name} has ref referenced type.");
        }
    }
}
