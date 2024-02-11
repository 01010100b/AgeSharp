using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Rules
{
    internal class Rule
    {
        public List<string> Comments { get; } = [];
        public List<string> Labels { get; } = [];
        public List<string> Facts { get; } = [];
        public List<string> Actions { get; } = [];
        public int Commands => Math.Max(1, Facts.Count) + Actions.Count;

        public bool IsAlwaysTrue()
        {
            if (Facts.Count == 0)
            {
                return true;
            }
            
            foreach (var fact in Facts)
            {
                if (fact.Contains("up-compare-goal"))
                {
                    return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            
            foreach (var comment in Comments)
            {
                sb.AppendLine($"; {comment}");
            }

            sb.AppendLine("(");

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

            foreach (var action in Actions)
            {
                sb.AppendLine($"\t({action})");
            }

            sb.AppendLine(")");

            return sb.ToString();
        }
    }
}
