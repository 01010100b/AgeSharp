﻿using System.Text;

namespace AgeSharp.Scripting.Compiler.Rules
{
    internal class Rule
    {
        public List<string> Comments { get; } = [];
        public List<string> Labels { get; } = [];
        public List<string> Facts { get; } = [];
        public List<string> Actions { get; } = [];
        public int CommandCount => Math.Max(1, Facts.Count) + Math.Max(1, Actions.Count);
        public bool IsEmpty => Facts.Count == 0 && Actions.Count == 0;
        public bool IsAlwaysTrue => Facts.Count == 0;
        public bool Jumps => Actions.Any(x => x.StartsWith("up-jump-"));

        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (var comment in Comments)
            {
                sb.AppendLine($"; {comment}");
            }

            sb.AppendLine("(defrule");

            if (Facts.Count == 0)
            {
                sb.AppendLine("\t(true)");
            }
            else
            {
                foreach (var fact in Facts)
                {
                    sb.AppendLine($"\t({fact})");
                }
            }

            sb.AppendLine("=>");

            if (Actions.Count == 0)
            {
                sb.AppendLine("\t(do-nothing)");
            }
            else
            {
                foreach (var action in Actions)
                {
                    sb.AppendLine($"\t({action})");
                }
            }

            sb.AppendLine(")");

            return sb.ToString();
        }
    }
}
