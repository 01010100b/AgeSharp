using System;
using System.Collections.Generic;
using System.Text;

namespace AgeSharp.ScriptGenerator.Attributes
{
    public class AgeMainMethodAttribute : AgeMethodAttribute
    {
        public string File { get; }

        public AgeMainMethodAttribute(string file)
        {
            File = file;
        }
    }
}
