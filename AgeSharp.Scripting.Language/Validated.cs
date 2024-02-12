using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language
{
    public abstract class Validated
    {
        public static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new Exception($"Invalid name {name}.");
        }

        public abstract void Validate();
    }
}
