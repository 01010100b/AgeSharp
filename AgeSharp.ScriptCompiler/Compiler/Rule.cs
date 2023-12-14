using System;
using System.Collections.Generic;
using System.Text;

namespace AgeSharp.ScriptCompiler.Compiler
{
    public class Rule
    {
        public List<string> Labels { get; } = new List<string>();
        public List<Command> Facts { get; } = new List<Command>();
        public List<Command> Actions { get; } = new List<Command>();

        public int Elements => Actions.Count + Math.Max(1, Facts.Count);
        public bool AlwaysTrue => Facts.Count == 0;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("(defrule");

            foreach (var fact in Facts)
            {
                sb.AppendLine($"    ({fact})");
            }

            sb.AppendLine("=>");

            foreach (var action in Actions)
            { 
                sb.AppendLine($"    ({action})");
            }

            sb.AppendLine(")");

            return sb.ToString();
        }
    }
}
