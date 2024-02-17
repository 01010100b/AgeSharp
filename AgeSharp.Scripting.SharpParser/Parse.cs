using AgeSharp.Scripting.Language;
using AgeSharp.Scripting.Language.Statements;
using AgeSharp.Scripting.Language.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Type = AgeSharp.Scripting.Language.Type;

namespace AgeSharp.Scripting.SharpParser
{
    internal class Parse(Script script, Compilation compilation)
    {
        public Script Script { get; } = script;
        public Compilation Compilation { get; } = compilation;
        private Dictionary<ITypeSymbol, Type> Types { get; } = [];
        private Dictionary<IFieldSymbol, Variable> Globals { get; } = [];
        private Dictionary<IMethodSymbol, Method> Methods { get; } = [];
        private Dictionary<IParameterSymbol, Variable> Parameters { get; } = [];
        private Dictionary<ILocalSymbol, Variable> Locals { get; } = [];

        public void AddType(ITypeSymbol symbol, Type type)
        {
            if (symbol.TypeKind == TypeKind.Array)
            {
                return;
            }

            Types.Add(symbol, type);
        }

        public Type GetType(ITypeSymbol symbol, int length = 1)
        {
            if (symbol.TypeKind == TypeKind.Array)
            {
                var arr = (IArrayTypeSymbol)symbol;
                var etype = GetType(arr.ElementType);

                return Script.GetOrAddArrayType(etype, length);
            }
            else
            {
                return Types[symbol];
            }
        }

        public void AddGlobal(IFieldSymbol symbol, Variable variable)
        {
            Globals.Add(symbol, variable);
        }

        public Variable GetGlobal(IFieldSymbol symbol)
        {
            return Globals[symbol];
        }

        public void AddMethod(IMethodSymbol symbol, Method method)
        {
            Methods.Add(symbol, method);
        }

        public Method GetMethod(IMethodSymbol symbol)
        {
            return Methods[symbol];
        }

        public void AddParameter(IParameterSymbol symbol, Variable variable)
        {
            Parameters.Add(symbol, variable);
        }

        public Variable GetParameter(IParameterSymbol symbol)
        {
            return Parameters[symbol];
        }

        public void AddLocal(ILocalSymbol symbol, Variable variable)
        {
            Locals.Add(symbol, variable);
        }

        public Variable GetLocal(ILocalSymbol symbol)
        {
            return Locals[symbol];
        }
    }
}
