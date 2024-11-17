namespace AgeSharp.Scripting.SharpParser
{
    public readonly struct Array<T>
    {
        public ref T this[Int i]
        {
            get => throw new Exception();
        }

        public Int Length { get; }

        public Array(Int length)
        {
            throw new Exception();
        }
    }
}
