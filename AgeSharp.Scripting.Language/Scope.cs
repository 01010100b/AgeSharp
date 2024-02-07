using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language
{
    public class Scope : Validated
    {
        public Scope? Parent { get; }
        public IReadOnlyList<Variable> Variables { get; } = new List<Variable>();

        internal Scope(Scope? parent) : base()
        {
            Parent = parent;
        }

        public Variable CreateVariable(string name, Type type, bool is_ref)
        {
            var variable = new Variable(this, name, type, is_ref);
            ((List<Variable>)Variables).Add(variable);

            return variable;
        }

        public override void Validate()
        {
            var in_scope = new HashSet<string>();
            var current = Parent;

            while (current is not null)
            {
                foreach (var variable in current.Variables)
                {
                    in_scope.Add(variable.Name);
                }

                current = current.Parent;
            }

            foreach (var variable in Variables)
            {
                variable.Validate();
                if (in_scope.Contains(variable.Name)) throw new Exception($"Variable {variable.Name} already in scope.");
                if (Parent is null && variable.IsRef) throw new Exception($"Global variable {variable.Name} is ref.");
            }
        }
    }
}
