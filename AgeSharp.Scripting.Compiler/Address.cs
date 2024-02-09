using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler
{
    internal class Address
    {
        public int Base { get; set; }
        public bool RefBase { get; set; }
        public int Offset { get; set; }
        public bool RefOffset { get; set; }
    }
}
