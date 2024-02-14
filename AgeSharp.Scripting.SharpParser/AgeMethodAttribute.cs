using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.SharpParser
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AgeMethodAttribute : Attribute
    {
        public bool EntryPoint { get; set; } = false;
    }
}
