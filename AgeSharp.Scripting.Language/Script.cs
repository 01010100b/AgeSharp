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
        public List<Type> Types { get; } = [];
        public List<Method> Methods { get; } = [];
        public Method? EntryPoint { get; private set; } = null;
        public IEnumerable<Scope> Scopes => Methods.SelectMany(x => x.GetAllBlocks().Select(x => x.Scope)).Append(GlobalScope);
        public IEnumerable<Variable> Variables => Scopes.SelectMany(x => x.Variables);

        public Script()
        {
            Types.Add(PrimitiveType.Int);
            Types.Add(PrimitiveType.Bool);
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
    }
}
