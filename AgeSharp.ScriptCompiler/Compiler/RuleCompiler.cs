using System;
using System.Collections.Generic;
using System.Text;

namespace AgeSharp.ScriptCompiler.Compiler
{
    public static class RuleCompiler
    {
        public static List<Rule> Compile(List<Instruction> instructions, int elements)
        {
            if (elements < 4)
            {
                throw new ArgumentOutOfRangeException(nameof(elements));
            }

            var rules = new List<Rule>() { new Rule() };

            foreach (var instruction in instructions)
            {
                var rule = rules[rules.Count - 1];

                if (rule.Elements >= elements)
                {
                    rule = new Rule();
                    rules.Add(rule);
                }

                if (instruction is Label label)
                {
                    if (rule.Actions.Count > 0)
                    {
                        rule = new Rule();
                        rules.Add(rule);
                    }

                    rule.Labels.Add(label.Id);
                }
                else if (instruction is JumpIfZero jump)
                {
                    if (rule.Actions.Count > 0)
                    {
                        rule = new Rule();
                        rules.Add(rule);
                    }

                    rule.Facts.Add(new Command("up-compare-goal", jump.Goal, "c:==", "0"));
                    rule.Actions.Add(new Command("up-jump-direct", "c:", jump.LabelId));
                    rule = new Rule();
                    rules.Add(rule);
                }
                else if (instruction is Command command)
                {
                    rule.Actions.Add(command);

                    if (command.Code.StartsWith("up-jump-"))
                    {
                        rule = new Rule();
                        rules.Add(rule);
                    }
                }
                else if (instruction is JumpReturnNext jumpret)
                {
                    if (rule.Elements >= elements - 3)
                    {
                        rule = new Rule();
                        rules.Add(rule);
                    }

                    rule.Actions.Add(new Command("up-get-rule-id", jumpret.ReturnAddressGoal));
                    rule.Actions.Add(new Command("up-modify-goal", jumpret.ReturnAddressGoal, "c:+", "1"));
                    rule.Actions.Add(new Command("up-jump-direct", "c:", jumpret.LabelId));
                    rule = new Rule();
                    rules.Add(rule);
                }
                else
                {
                    throw new Exception("Instruction not recognized.");
                }
            }

            return rules;
        }
    }
}
