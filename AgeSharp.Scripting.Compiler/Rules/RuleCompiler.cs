﻿using AgeSharp.Scripting.Compiler.Instructions;
using System.Diagnostics;

namespace AgeSharp.Scripting.Compiler.Rules
{
    internal class RuleCompiler(IEnumerable<Instruction> instructions, Settings settings)
    {
        private List<Instruction> Instructions { get; } = instructions.ToList();
        private Settings Settings { get; } = settings;

        public List<Rule> Compile()
        {
            var rules = new List<Rule>();
            var current = new Rule();

            foreach (var instruction in Instructions)
            {
                if (current.CommandCount >= Settings.MaxRuleCommands)
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
                else if (instruction is JumpFactInstruction jf)
                {
                    if (!current.IsEmpty)
                    {
                        rules.Add(current);
                        current = new();
                    }

                    current.Facts.Add(jf.Fact);
                    current.Actions.Add($"up-jump-direct c: {jf.Label.Label}");
                    rules.Add(current);
                    current = new();
                }
                else if (instruction is RuleInstruction rule)
                {
                    if (!current.IsEmpty)
                    {
                        rules.Add(current);
                        current = new();
                    }

                    Debug.Assert(rule.Facts.Count + rule.Actions.Count <= Settings.MaxRuleCommands);

                    current.Facts.AddRange(rule.Facts);
                    current.Actions.AddRange(rule.Actions);
                    rules.Add(current);
                    current = new();
                }
                else
                {
                    throw new NotImplementedException($"Instruction {instruction.GetType().Name} not recognized.");
                }
            }

            rules.Add(current);

            return rules;
        }
    }
}
