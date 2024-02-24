using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler
{
    public class Settings : Validated
    {
        public int MinGoal { get; set; } = 1;
        public int MaxGoal { get; set; } = 512;
        public int MaxRuleCommands { get; set; } = 16;
        public bool CompileUnusedMethods { get; set; } = true;

        public override void Validate()
        {
        }
    }
}
