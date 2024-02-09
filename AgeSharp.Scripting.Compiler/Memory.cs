using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler
{
    internal class Memory
    {
        public int StackPtr { get; }
        public int RegisterCount { get; }

        public void Compile(Script script, Settings settings)
        {

        }

        public Address GetAddress(Variable variable)
        {
            throw new NotImplementedException();
        }
    }
}
