using AgeSharp.Scripting.Language;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AgeSharp.Scripting.Compiler
{
    internal class Memory
    {
        public int Error { get; }
        public int MaxStackSpaceUsed { get; }
        public int StackPtr { get; }
        public int ConditionGoal { get; }
        public int ExpressionGoal { get; }
        public int AddressingGoal { get; }
        public int MemCpy3 { get; }
        public int MemCpy2 { get; }
        public int MemCpy1 { get; }
        public int MemCpy0 { get; }
        public int CallResultCount { get; }
        public int CallResultBase { get; }
        public int RegisterCount { get; }
        public int RegisterBase { get; }
        public int GlobalCount { get; }
        public int GlobalBase { get; }
        public int StackLimit => GlobalBase;
        public int InitialStackPtr { get; }
        
        private Dictionary<Variable, Address> VariableAddresses { get; } = [];

        public Memory(Script script, Settings settings)
        {
            CallResultCount = script.Methods.Max(x => x.ReturnType.Size);
            GlobalCount = script.GlobalScope.Variables.Sum(x => x.Size);
            RegisterCount = script.Methods.Max(GetRegisterCount);

            var goal = settings.MaxGoal;
            Error = goal--;
            MaxStackSpaceUsed = goal--;
            StackPtr = goal--;
            ConditionGoal = goal--;
            ExpressionGoal = goal--;
            AddressingGoal = goal--;
            MemCpy3 = goal--;
            MemCpy2 = goal--;
            MemCpy1 = goal--;
            MemCpy0 = goal--;

            goal -= CallResultCount;
            CallResultBase = goal;
            goal -= RegisterCount;
            RegisterBase = goal;
            goal -= GlobalCount;
            GlobalBase = goal;
            InitialStackPtr = settings.MinGoal;

            if (goal < settings.MinGoal) throw new NotSupportedException("Not enough goal memory.");

            SetVariableAddresses(script, settings);
        }

        public Address GetAddress(Variable variable)
        {
            if (VariableAddresses.TryGetValue(variable, out var address))
            {
                Debug.Assert(address.Goal > 0);
                Debug.Assert(address.Offset == 0);
                Debug.Assert(address.IndexStride == 0);

                return address;
            }
            else
            {
                throw new NotSupportedException($"Variable {variable} has no address.");
            }
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

        private void SetVariableAddresses(Script script, Settings settings)
        {
            VariableAddresses.Clear();

            foreach (var scope in script.Scopes)
            {
                if (scope == script.GlobalScope)
                {
                    var offset = 0;

                    foreach (var variable in scope.Variables)
                    {
                        var addr = new Address(GlobalBase + offset, false);
                        VariableAddresses.Add(variable, addr);
                        offset += variable.Size;
                    }
                }
                else
                {
                    var offset = scope.FullSize - scope.Size - script.GlobalScope.Size;

                    foreach (var variable in scope.Variables)
                    {
                        var addr = new Address(RegisterBase + 1 + offset, false);
                        VariableAddresses.Add(variable, addr);
                        offset += variable.Size;
                    }
                }
            }
        }
    }
}
