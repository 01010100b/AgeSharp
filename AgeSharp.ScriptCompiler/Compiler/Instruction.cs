using System;
using System.Collections.Generic;
using System.Text;

namespace AgeSharp.ScriptCompiler.Compiler
{
    public abstract class Instruction
    {
    }

    public class Command : Instruction
    {
        public string Code { get; }
        public string Arg0 { get; }
        public string Arg1 { get; }
        public string Arg2 { get; }
        public string Arg3 { get; }

        public Command(string code, string arg0 = "", string arg1 = "", string arg2 = "", string arg3 = "")
        {
            Code = code;
            Arg0 = arg0;
            Arg1 = arg1;
            Arg2 = arg2;
            Arg3 = arg3;
        }

        public override string ToString()
        {
            return $"{Code} {Arg0} {Arg1} {Arg2} {Arg3}".Trim();
        }
    }

    public class Label : Instruction
    {
        public string Id { get; }

        public Label(string id)
        {
            Id = id;
        }
    }

    public class JumpZero : Instruction
    {
        public string Goal { get; }
        public string LabelId { get; }

        public JumpZero(string goal, string label_id)
        {
            Goal = goal;
            LabelId = label_id;
        }
    }

    public class JumpReturnNext : Instruction
    {
        public string ReturnAddressGoal { get; }
        public string LabelId { get; }

        public JumpReturnNext(string return_address_goal, string label_id)
        {
            ReturnAddressGoal = return_address_goal;
            LabelId = label_id;
        }
    }
}
