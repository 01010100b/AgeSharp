using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AgeSharp.Scripting.Compiler
{
    internal class Memory
    {
        public int StackPtr { get; }
        public int ConditionGoal { get; }
        public int RegisterBase { get; }
        public int GlobalBase { get; }
        public int RegisterCount { get; }
        public int GlobalCount { get; }

        private Dictionary<Variable, Address> VariableAddresses { get; } = [];

        public Memory(Script script, Settings settings)
        {
            GlobalCount = script.GlobalScope.Variables.Sum(x => x.Size);
            RegisterCount = script.Methods.Max(GetRegisterCount);

            var goal = settings.MaxGoal;
            StackPtr = goal--;
            ConditionGoal = goal--;

            goal -= RegisterCount;
            RegisterBase = goal;
            goal -= GlobalCount;
            GlobalBase = goal;

            if (goal < settings.MinGoal) throw new Exception("Not enough goal memory.");
        }

        public Address GetAddress(Variable variable)
        {
            throw new NotImplementedException();
        }

        public int GetRegisterCount(Method method)
        {
            if (method is Intrinsic)
            {
                return 0;
            }

            // first register is return addr

            var max = 0;

            foreach (var block in method.GetAllBlocks())
            {
                max = Math.Max(max, 1 + block.Scope.GetAllScopedVariables().Sum(x => x.Size));
            }

            return max - GlobalCount;
        }
    }
}
