using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language
{
    public abstract class Expression : Validated
    {
        public abstract Type? Type { get; }
    }
}
