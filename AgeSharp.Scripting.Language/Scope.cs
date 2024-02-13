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
        public Scope? Parent { get; private set; }
        public IReadOnlyList<Variable> Variables { get; } = new List<Variable>();
        public int Size => Variables.Sum(x => x.Size);
        public int FullSize => GetAllScopedVariables().Sum(x => x.Size);

        internal Scope(Scope? parent) : base()
        {
            Parent = parent;
        }

        public void Rebase(Scope parent)
        {
            Parent = parent;
        }

        public void AddVariable(Variable variable)
        {
            if (GetAllScopedVariables().Select(x => x.Name).Contains(variable.Name)) throw new NotSupportedException($"Variable {variable.Name} already in scope.");

            ((List<Variable>)Variables).Add(variable);
        }

        public IEnumerable<Variable> GetAllScopedVariables()
        {
            var current = this;

            while (current is not null)
            {
                foreach (var variable in current.Variables)
                {
                    yield return variable;
                }

                current = current.Parent;
            }
        }

        public bool IsInScope(Variable variable) => GetAllScopedVariables().Contains(variable);

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
