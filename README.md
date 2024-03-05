# AgeSharp

AgeSharp is a set of .NET tools for Age of Empires 2 with a focus on AI scripting. For now that is limited to a custom .per compiler and a C# parser for that compiler.

## AgeSharp.Scripting

The ```AgeSharp.Scripting``` namespace contains all the tools related to the custom .per compiler. ```AgeSharp.Scripting.Language``` defines an abstract representation of an AI script as an instance of the ```Script``` class. The ```ScriptCompiler``` class in ```AgeSharp.Scripting.Compiler``` can then be used to compile a script to its .per string. The standard included parser is a C# parser based on Roslyn.

### AgeSharp.Scripting.SharpParser

This is the standard C# parser. It takes C# source code and produces an instance of the ```Script``` class to be compiled. While having many features, there are also quite some limitations. In particular there is no support for reference types, even implicitly such as with the ```foreach``` construct taking an enumerator. C# source code should be written using ```AgeSharp.Scripting.SharpParser``` as a library, which includes builtin types such as ```Int``` or ```Bool``` as well as available intrinsics in the ```Intrinsics``` class.

#### Types

There are several kinds of types: primitive types, array types, and compound types. The type system is closed, any code can only ever refer to types marked as ```AgeType``` or the builtin types, with the exception of the use of ```void``` as a method return type. Some of the builtin types such as ```Int``` and ```Bool``` have appropriate cast operators defined to correspond to the C# ```int``` and ```bool``` types.

Custom compound types can be defined as C# structs marked with the ```AgeType``` attribute. For example:

```
[AgeType]
internal struct Group
{
    public Int Id;
    public Int Type;
    private Int Count;
}
```

Only instance fields are supported, not properties or other constructs. Arrays can not be fields.

#### Globals

A global variable is defined by marking a static field with the ```AgeGlobal``` attribute. For example:

```
internal class Main
{
	[AgeGlobal]
	private static Group MyGroup;
}
```

