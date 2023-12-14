using System;
using System.Collections.Generic;
using System.Text;

namespace AgeSharp.ScriptCompiler.Language.Statements
{
    public class Block : Statement
    {
        public Scope Scope { get; }
        public List<Statement> Statements { get; } = new List<Statement>();

        public Block(Script script, Block parent)
        {
            if (parent != null)
            {
                Scope = new Scope(parent.Scope);
            }
            else
            {
                Scope = new Scope(script.GlobalScope);
            }
        }
    }
}
