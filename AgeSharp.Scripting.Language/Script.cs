using AgeSharp.Scripting.Language.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language
{
    public class Script : Validated
    {
        public Scope GlobalScope { get; } = new(null);
        public IReadOnlyList<Type> Types { get; } = new List<Type>();
        public IReadOnlyList<Method> Methods { get; } = new List<Method>();
        public Method? EntryPoint { get; private set; } = null;
        public IEnumerable<Scope> Scopes => Methods.SelectMany(x => x.GetAllBlocks().Select(x => x.Scope)).Append(GlobalScope);
        public IEnumerable<Variable> Variables => Scopes.SelectMany(x => x.Variables);

        public Script()
        {
            AddType(PrimitiveType.Int);
            AddType(PrimitiveType.Bool);

            var point = CreateCompoundType("Point");
            point.Fields.Add(new("X", PrimitiveType.Int));
            point.Fields.Add(new("Y", PrimitiveType.Int));

            var cost = CreateCompoundType("Cost");
            cost.Fields.Add(new("Food", PrimitiveType.Int));
            cost.Fields.Add(new("Wood", PrimitiveType.Int));
            cost.Fields.Add(new("Stone", PrimitiveType.Int));
            cost.Fields.Add(new("Gold", PrimitiveType.Int));
        }

        public ArrayType CreateArrayType(Type element_type, int length)
        {
            var t = new ArrayType(element_type, length);
            AddType(t);

            return t;
        }

        public CompoundType CreateCompoundType(string name)
        {
            var t = new CompoundType(name);
            AddType(t);

            return t;
        }

        public Method CreateMethod(string name, Type? return_type, bool entry_point = false)
        {
            var m = new Method(this, name, return_type);
            AddMethod(m);

            if (entry_point)
            {
                if (EntryPoint is not null)
                {
                    throw new Exception($"Entry point already exists when adding method {name}.");
                }

                EntryPoint = m;
            }

            return m;
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
            if (EntryPoint.Parameters.Count > 0) throw new Exception($"Entry point {EntryPoint.Name} has parameters.");

        }

        private void AddType(Type type)
        {
            if (Types.Select(x => x.Name).Contains(type.Name)) throw new Exception($"Type {type.Name} already exists.");

            ((List<Type>)Types).Add(type);
        }

        private void AddMethod(Method method)
        {
            if (Methods.Select(x => x.Name).Contains(method.Name)) throw new Exception($"Method {method.Name} already exists.");

            ((List<Method>)Methods).Add(method);
        }
    }
}
