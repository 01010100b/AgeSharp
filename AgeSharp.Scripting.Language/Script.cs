using AgeSharp.Scripting.Language.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language
{
    public class Script : Validated
    {
        public Scope GlobalScope { get; } = new(null);
        public IEnumerable<Type> Types { get; } = new List<Type>();
        public IEnumerable<Method> Methods { get; } = new List<Method>();
        public Method? EntryPoint { get; private set; } = null;
        public IEnumerable<Scope> Scopes => Methods.SelectMany(x => x.GetAllBlocks().Select(x => x.Scope)).Append(GlobalScope);
        public IEnumerable<Variable> Variables => Scopes.SelectMany(x => x.Variables);

        public Script()
        {
            AddType(PrimitiveType.Int);
            AddType(PrimitiveType.Bool);
        }

        public void AddType(Type type)
        {
            if (Types.Select(x => x.Name).Contains(type.Name)) throw new Exception($"Type {type.Name} already exists.");

            ((List<Type>)Types).Add(type);
        }

        public void AddMethod(Method method, bool entry_point = false)
        {
            if (Methods.Select(x => x.Name).Contains(method.Name)) throw new Exception($"Method {method.Name} already exists.");
            if (entry_point && EntryPoint is not null) throw new Exception($"Already have entry point {EntryPoint.Name}.");

            ((List<Method>)Methods).Add(method);

            if (entry_point)
            {
                EntryPoint = method;
            }
        }

        public override void Validate()
        {
            GlobalScope.Validate();

            foreach (var type in Types)
            {
                type.Validate();
            }

            foreach (var method in Methods)
            {
                method.Validate();
            }

            if (EntryPoint is null) throw new Exception("No entry point.");
            if (EntryPoint.ReturnType is not null) throw new Exception($"Entry point {EntryPoint.Name} has non-void return type.");
            if (EntryPoint.Parameters.Any()) throw new Exception($"Entry point {EntryPoint.Name} has parameters.");

        }
    }
}
