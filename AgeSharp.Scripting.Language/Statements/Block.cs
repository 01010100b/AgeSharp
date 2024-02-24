using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AgeSharp.Scripting.Language.Statements
{
    public class Block(Scope scope) : Statement
    {
        public override Scope Scope { get; } = new(scope);
        public List<Statement> Statements { get; } = [];

        public override IEnumerable<Block> GetContainedBlocks()
        {
            yield return this;
        }

        public override IEnumerable<Expression> GetContainedExpressions() => Enumerable.Empty<Expression>();

        public IEnumerable<Block> GetChildBlocks()
        {
            foreach (var statement in Statements)
            {
                foreach (var block in statement.GetContainedBlocks())
                {
                    yield return block;
                }
            }
        }

        public override void Validate()
        {
            Scope.Validate();

            for (int i = 0; i < Statements.Count; i++)
            {
                var statement = Statements[i];
                statement.Validate();

                if (statement is ReturnStatement && i != Statements.Count - 1) throw new NotSupportedException($"Return statement not last statement of block.");
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");

            foreach (var local in Scope.Variables)
            {
                sb.AppendLine($"\t{local}");
            }

            foreach (var statement in Statements)
            {
                var lines = Regex.Split(statement.ToString()!, "\r|\n|\r\n");

                foreach (var line in lines.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    sb.AppendLine($"\t{line}");
                }
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
