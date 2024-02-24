using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        }
    }
}
