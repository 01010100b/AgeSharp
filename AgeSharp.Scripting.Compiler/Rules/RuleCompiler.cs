using AgeSharp.Scripting.Compiler.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Compiler.Rules
{
    internal class RuleCompiler(IEnumerable<Instruction> instructions, Settings settings)
    {
        private IEnumerable<Instruction> Instructions { get; } = instructions.ToList();
        private Settings Settings { get; } = settings;

        public List<Rule> Compile()
        { 
            var rules = new List<Rule>();
            var current = new Rule();

            foreach (var instruction in Instructions)
            {
                if (current.Commands >= Settings.MaxRuleCommands)
                {
                    rules.Add(current);
                    current = new();
                }

                if (instruction is LabelInstruction label)
                {
                    if (!current.IsEmpty)
                    {
                        rules.Add(current);
                        current = new();
                    }

                    current.Labels.Add(label.Label);
                }
                else if (instruction is CommandInstruction command)
                {
                    current.Actions.Add(command.Command);
                }
                else if (instruction is JumpInstruction jump)
                {
                    current.Actions.Add($"up-jump-direct c: {jump.Label.Label}");
                    rules.Add(current);
                    current = new();
                }
                else if (instruction is JumpIndirectInstruction ji)
                {
                    current.Actions.Add($"up-jump-direct g: {ji.Goal}");
                    rules.Add(current);
                    current = new();
                }
                else if (instruction is JumpConditional jc)
                {
                    rules.Add(current);
                    current = new();
                    current.Facts.Add($"up-compare-goal {jc.Goal} c:{jc.Comparison} {jc.Value}");
                    current.Actions.Add($"up-jump-direct c: {jc.Label.Label}");
                    rules.Add(current);
                    current = new();
                }
                else
                {
                    throw new NotSupportedException($"Instruction {instruction.GetType().Name} not recognized.");
                }
            }

            rules.Add(current);

            return rules;
        }
    }
}
