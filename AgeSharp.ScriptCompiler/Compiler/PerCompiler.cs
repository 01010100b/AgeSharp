using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace AgeSharp.ScriptCompiler.Compiler
{
    public static class PerCompiler
    {
        public static string Compile(List<Rule> rules)
        {
            var sb = new StringBuilder();
            var labels = new Dictionary<string, int>();

            for (int i = 0; i < rules.Count; i++)
            {
                var rule = rules[i];

                foreach (var label in rule.Labels)
                {
                    labels.Add(label, i);
                }

                sb.AppendLine($"; {i}");
                sb.AppendLine(rule.ToString());
            }

            var per = sb.ToString();

            foreach (var kvp in labels)
            {
                per = per.Replace(kvp.Key, kvp.Value.ToString());
            }

            return per;
        }
    }
}
