using AgeSharp.ScriptGenerator.Attributes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

namespace AgeSharp.ScriptGenerator
{
    [Generator]
#pragma warning disable RS1036 // Specify analyzer banned API enforcement setting
    internal class Generator : ISourceGenerator
#pragma warning restore RS1036 // Specify analyzer banned API enforcement setting
    {
        private class DataReceiver : ISyntaxReceiver
        {
            public List<MethodDeclarationSyntax> Methods { get; } = new List<MethodDeclarationSyntax>();
            public List<TypeDeclarationSyntax> Types { get; } = new List<TypeDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode node)
            {
                if (node is MethodDeclarationSyntax method)
                {
                    Methods.Add(method);
                }
                else if (node is TypeDeclarationSyntax type)
                {
                    Types.Add(type);
                }
            }
        }

        public static void Log(string message)
        {
            const string LOG = @"F:\atest.txt";

            if (message is null)
            {
                File.Delete(LOG);
            }
            else
            {
                File.AppendAllText(LOG, message + "\n");
            }
        }

        public void Execute(GeneratorExecutionContext context)
        {
            Log(null);
            Log($"EXECUTE {DateTime.Now}");

            //var script = new Script();
            var receiver = (DataReceiver)context.SyntaxReceiver;

            TypeGenerator.Generate(context, receiver.Types);
            MethodGenerator.Generate(context, receiver.Methods);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new DataReceiver());
        }
    }
}
