using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.SharpParser
{
    public readonly struct Array<T>
    {
        public T this[Int i]
        {
            get => throw new Exception();
            set => throw new Exception();
        }

        public Array(Int length)
        {

        }
    }
}
