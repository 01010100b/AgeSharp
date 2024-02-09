﻿using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Compiler.Rules;
using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler
{
    public class CompilationResult
    {
        public int RuleCount => Rules.Count;
        public int CommandCount => Rules.Sum(x => x.Commands);

        internal Script Script { get; }
        internal Settings Settings { get; }
        internal Memory Memory { get; }
        internal IReadOnlyList<Instruction> Instructions { get; }
        internal IReadOnlyList<Rule> Rules { get; }

        internal CompilationResult(Script script, Settings settings, Memory memory, IReadOnlyList<Instruction> instructions, IReadOnlyList<Rule> rules)
        {
            Script = script;
            Settings = settings;
            Memory = memory;
            Instructions = instructions.ToList();
            Rules = rules.ToList();
        }

        public string GetPer()
        {
            var sb = new StringBuilder();
            var labels = new Dictionary<string, int>();

            for (int i = 0; i < Rules.Count; i++)
            {
                var rule = Rules[i];

                sb.AppendLine($"; {i}");
                sb.AppendLine(rule.ToString());

                foreach (var label in rule.Labels)
                {
                    labels.Add(label, i);
                }
            }

            var per = sb.ToString();

            foreach (var label in labels)
            {
                per = per.Replace(label.Key, label.Value.ToString());
            }

            return per;
        }
    }
}
