using AgeSharp.Common;
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
            if (symbol is not INamedTypeSymbol named)
            {
                throw new NotSupportedException($"Type {symbol.Name} is not named type.");
            }

            if (!IsArrayType(named))
            {
                Types.Add(symbol, type);
            }
        }

        public Type GetType(ITypeSymbol symbol, int length = 1)
        {
            if (symbol is not INamedTypeSymbol named)
            {
                throw new NotSupportedException($"Type {symbol.Name} is not named type.");
            }

            if (IsArrayType(named))
            {
                var etype = named.TypeArguments.Single();
                
                if (etype is not INamedTypeSymbol enamed)
                {
                    throw new NotSupportedException($"Array type {symbol.Name} when element type {etype.Name} is not named type.");
                }

                if (enamed.IsGenericType)
                {
                    throw new NotSupportedException($"Array type {symbol.Name} with generic element type {etype.Name}.");
                }

                Throw.If<NotSupportedException>(IsArrayType(enamed), $"Array type {symbol.Name} with element type also array.");

                return Script.GetArrayType(GetType(etype), length);
            }
            else if (Types.TryGetValue(named, out var type))
            {
                Debug.Assert(Script.Types.Contains(type));

                return type;
            }
            else if (named.TypeKind == TypeKind.Enum)
            {
                return PrimitiveType.Int;
            }
            else
            {
                throw new NotSupportedException($"Type {named.Name} not found, possibly missing AgeType attribute.");
            }
        }

        private bool IsArrayType(INamedTypeSymbol symbol)
        {
            if (!symbol.IsGenericType)
            {
                return false;
            }
            else if (!IsInternal(symbol))
            {
                return false;
            }
            else if (symbol.Name != "Array")
            {
                return false;
            }

            return true;
        }

        public void AddGlobal(IFieldSymbol symbol, Variable variable)
        {
            Globals.Add(symbol, variable);
        }

        public Variable GetGlobal(IFieldSymbol symbol)
        {
            if (Globals.TryGetValue(symbol, out var global))
            {
                Debug.Assert(Script.GlobalScope.Variables.Contains(global));

                return global;
            }
            else
            {
                throw new NotSupportedException($"Global {symbol.Name} not found, possibly missing AgeGlobal attribute.");
            }
        }

        public void AddMethod(IMethodSymbol symbol, Method method)
        {
            Methods.Add(symbol, method);
        }

        public Method GetMethod(IMethodSymbol symbol)
        {
            if (Methods.TryGetValue(symbol, out var method))
            {
                Debug.Assert(Script.Methods.Contains(method));

                return method;
            }
            else
            {
                throw new NotSupportedException($"Method {symbol.Name} not found, possibly missing AgeMethod attribute.");
            }
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

        public bool IsInternal(ISymbol symbol)
        {
            var namesp = symbol.ContainingNamespace;

            if (namesp is null)
            {
                return false;
            }
            else if (namesp.Name != "SharpParser")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
