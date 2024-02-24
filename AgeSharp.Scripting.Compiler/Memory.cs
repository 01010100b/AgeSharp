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
        public int Utils6 { get; }
        public int Utils5 { get; }
        public int Utils4 { get; }
        public int Utils3 { get; }
        public int Utils2 { get; }
        public int Utils1 { get; }
        public int Utils0 { get; }
        public int Intr4 { get; }
        public int Intr3 {  get; }
        public int Intr2 { get; }
        public int Intr1 { get; }
        public int Intr0 { get; }
        public int CallResultCount { get; }
        public int CallResultBase { get; }
        public int RegistersCount { get; }
        public int RegistersBase { get; }
        public int GlobalsCount { get; }
        public int GlobalsBase { get; }
        public int InitialStackPtr { get; }

        public int StackLimit => GlobalsBase;
        public int ReturnAddressGoal => RegistersBase;

        private Dictionary<Variable, Address> VariableAddresses { get; } = [];

        public Memory(Script script, Settings settings)
        {
            CallResultCount = script.Methods.Max(x => x.ReturnType.Size);
            GlobalsCount = script.GlobalScope.Variables.Sum(x => x.Size);
            RegistersCount = script.Methods.Max(GetRegisterCount);

            var goal = settings.MaxGoal;
            Error = goal--;
            MaxStackSpaceUsed = goal--;
            StackPtr = goal--;
            ConditionGoal = goal--;
            ExpressionGoal = goal--;
            Utils6 = goal--;
            Utils5 = goal--;
            Utils4 = goal--;
            Utils3 = goal--;
            Utils2 = goal--;
            Utils1 = goal--;
            Utils0 = goal--;
            Intr4 = goal--;
            Intr3 = goal--;
            Intr2 = goal--;
            Intr1 = goal--;
            Intr0 = goal--;

            goal -= CallResultCount;
            CallResultBase = goal;
            goal -= RegistersCount;
            RegistersBase = goal;
            goal -= GlobalsCount;
            GlobalsBase = goal;
            InitialStackPtr = settings.MinGoal;

            if (goal < settings.MinGoal) throw new NotSupportedException("Not enough goal memory.");

            SetVariableAddresses(script, settings);
        }

        public Address GetAddress(Variable variable)
        {
            if (VariableAddresses.TryGetValue(variable, out var address))
            {
                Debug.Assert(address.Goal > 0);
                Debug.Assert(!address.IsRef);
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

            return max - GlobalsCount;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"CallResultCount {CallResultCount}");
            sb.AppendLine($"CallResultBase {CallResultBase}");
            sb.AppendLine($"RegisterCount {RegistersCount}");
            sb.AppendLine($"RegisterBase {RegistersBase}");
            sb.AppendLine($"GlobalsCount {GlobalsCount}");
            sb.AppendLine($"GlobalsBase {GlobalsBase}");
            sb.AppendLine($"StackLimit {StackLimit}");
            sb.AppendLine($"InitialStackPtr {InitialStackPtr}");

            sb.AppendLine($"Globals:");
            var globals = VariableAddresses
                .Where(x => x.Value.Goal >= GlobalsBase && x.Value.Goal < GlobalsBase + GlobalsCount)
                .OrderByDescending(x => x.Value.Goal)
                .ToList();

            foreach (var global in globals)
            {
                sb.AppendLine($"{global.Value.Goal}: {global.Key}");
            }

            return sb.ToString();
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
                        var addr = new Address(variable.Type, GlobalsBase + offset, false);
                        VariableAddresses.Add(variable, addr);
                        offset += variable.Size;
                    }
                }
                else
                {
                    var offset = scope.FullSize - scope.Size - script.GlobalScope.Size;

                    foreach (var variable in scope.Variables)
                    {
                        var addr = new Address(variable.Type, RegistersBase + 1 + offset, false);
                        VariableAddresses.Add(variable, addr);
                        offset += variable.Size;
                    }
                }
            }
        }
    }
}
