namespace AgeSharp.Scripting.Compiler.Instructions
{
    internal static class InstructionOptimizer
    {
        public static void Optimize(List<Instruction> instructions)
        {
            for (int i = 0; i < 3; i++)
            {
                RunPass(instructions);
            }
        }

        private static void RunPass(List<Instruction> instructions)
        {
            RemovePointlessJumps(instructions);
            RemoveUnusedLabels(instructions);
        }

        private static void RemoveUnusedLabels(List<Instruction> instructions)
        {
            var labels = new HashSet<LabelInstruction>();

            foreach (var label in instructions.OfType<LabelInstruction>())
            {
                labels.Add(label);
            }

            foreach (var instruction in instructions)
            {
                if (instruction is CommandInstruction command)
                {
                    foreach (var label in labels.ToList())
                    {
                        if (command.Command.Contains(label.Label))
                        {
                            labels.Remove(label);
                        }
                    }
                }
                else if (instruction is JumpFactInstruction jf)
                {
                    foreach (var label in labels.ToList())
                    {
                        if (label == jf.Label)
                        {
                            labels.Remove(label);
                        }
                    }
                }
                else if (instruction is JumpInstruction j)
                {
                    foreach (var label in labels.ToList())
                    {
                        if (label == j.Label)
                        {
                            labels.Remove(label);
                        }
                    }
                }
                else if (instruction is RuleInstruction rule)
                {
                    foreach (var label in labels.ToList())
                    {
                        foreach (var c in rule.Facts.Concat(rule.Actions))
                        {
                            if (c.Contains(label.Label))
                            {
                                labels.Remove(label);
                            }
                        }
                    }
                }
            }

            instructions.RemoveAll(labels.Contains);
        }

        private static void RemovePointlessJumps(List<Instruction> instructions)
        {
            for (int i = 0; i < instructions.Count; i++)
            {
                if (instructions[i] is not JumpInstruction jump)
                {
                    continue;
                }

                var pointless = false;

                for (int j = i + 1; j < instructions.Count; j++)
                {
                    if (instructions[j] is not LabelInstruction label)
                    {
                        break;
                    }

                    if (label == jump.Label)
                    {
                        pointless = true;
                    }
                }

                if (pointless)
                {
                    instructions.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
