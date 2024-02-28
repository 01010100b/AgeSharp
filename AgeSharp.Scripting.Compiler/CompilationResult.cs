using AgeSharp.Scripting.Compiler.Instructions;
using AgeSharp.Scripting.Compiler.Rules;
using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Expressions;
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
        public int CommandCount => Rules.Sum(x => x.CommandCount);
        public IEnumerable<string> InstructionStream => Instructions.Select(x => x.ToString());

        private Script Script { get; }
        private Settings Settings { get; }
        private Memory Memory { get; }
        private IReadOnlyList<Instruction> Instructions { get; }
        private IReadOnlyList<Rule> Rules { get; }

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

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"### SCRIPT ###");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(Script.ToString());

            sb.AppendLine("### MEMORY ###");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(Memory.ToString());
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("### INTR CALL COUNTS ###");
            sb.AppendLine();
            sb.AppendLine();

            var intrcalls = new Dictionary<Intrinsic, int>();

            foreach (var intr in Script.Methods
                .SelectMany(x => x.GetAllBlocks())
                .SelectMany(x => x.Statements)
                .SelectMany(x => x.GetContainedExpressions())
                .OfType<CallExpression>()
                .Select(x => x.Method)
                .OfType<Intrinsic>())
            {
                if (!intrcalls.ContainsKey(intr))
                {
                    intrcalls.Add(intr, 0);
                }

                intrcalls[intr]++;
            }

            foreach (var kvp in intrcalls.OrderByDescending(x => x.Value))
            {
                sb.AppendLine($"{kvp.Key.Name} {kvp.Value}");
            }

            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine(@"### INSTRUCTIONS ###");
            sb.AppendLine();
            sb.AppendLine();

            foreach (var instruction in Instructions)
            {
                sb.AppendLine(instruction.ToString());
            }

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("### PER ###");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"Using {RuleCount:N0} rules and {CommandCount:N0} commands ({CommandCount / (double)RuleCount:N2} commands/rule)");
            sb.AppendLine();
            sb.AppendLine(GetPer());

            return sb.ToString();
        }
    }
}
