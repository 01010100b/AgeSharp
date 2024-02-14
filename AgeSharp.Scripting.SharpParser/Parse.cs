using AgeSharp.Scripting.Language;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Type = AgeSharp.Scripting.Language.Type;

namespace AgeSharp.Scripting.SharpParser
{
    internal class Parse(Script script, Compilation compilation)
    {
        public Script Script { get; } = script;
        public Compilation Compilation { get; } = compilation;
        public Dictionary<INamedTypeSymbol, Type> Types { get; } = [];
        public Dictionary<IFieldSymbol, Variable> Globals { get; } = [];
        public Dictionary<IMethodSymbol, Method> Methods { get; } = [];
        public Dictionary<ILocalSymbol, Variable> Locals { get; } = [];
    }
}
