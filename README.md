# AgeSharp

AgeSharp is a set of .NET tools for Age of Empires 2 with a focus on AI scripting. For now that is limited to a custom .per compiler and a C# parser for that compiler.

## AgeSharp.Scripting

The ```AgeSharp.Scripting``` namespace contains all the tools related to the custom .per compiler. ```AgeSharp.Scripting.Language``` defines an abstract representation of an AI script as a valid instance of the ```Script``` class. The ```ScriptCompiler``` class in ```AgeSharp.Scripting.Compiler``` can then be used to compile a script to its .per string. The standard included parser is a C# parser based on Roslyn, though it's possible to implement different parsers for different languages as long as they can output a valid instance of the ```Script``` class.

The compiler automatically employs workarounds for some known bugs in the game's scripting engine. For example the use of ```up-get-player-fact``` when the fact is ```player-in-game``` is currently bugged in DE, but a workaround is employed transparently to the user so user code can still just call ```GetPlayerFact``` with that particular fact. Not all bugs have workarounds implemented, but more are added over time. If the bugs in question ever get fixed then the workaround can be removed in the compiler without the user ever having to do or change anything about their own code.

### AgeSharp.Scripting.SharpParser

This is the standard C# parser. It takes C# source code and produces an instance of the ```Script``` class to be compiled. While having many features, there are also quite some restrictions. In particular there is no support for reference types, even implicitly such as with the ```foreach``` construct taking an enumerator or things like the ```typeof``` operator returning a reference type. C# source code should be written using ```AgeSharp.Scripting.SharpParser``` as an API, which includes builtin types such as ```Int``` or ```Bool``` as well as available intrinsics in the ```Intrinsics``` class.

#### Types

There are several kinds of types: primitive types, array types, and compound types. There are also ref types but these are restricted to method parameters. The type system is closed, any code can only ever refer to types marked as ```AgeType``` or the builtin types, with the exception of the use of ```void``` as a method return type. Some of the builtin types such as ```Int``` and ```Bool``` have appropriate cast and other operators defined to correspond to the C# ```int``` and ```bool``` types. Array types have a special readonly ```Length``` field. Custom compound types can be defined as C# structs marked with the ```AgeType``` attribute. For example:

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
	[AgeGlobal]
	public static Array<Group> MyGroupArray = new(10);
}
```

Arrays must have an initializer and the length must be a compile-time constant. Other global variables may not have a initializer. All global variables, including all array elements, are initialized to 0.

#### Methods

Methods are defined by marking a method with the ```AgeMethod``` attribute. For example:

```
internal class MyClass
{
	[AgeMethod]
	public static Int IncrementId(ref Group group, Int value)
	{
		group.Id += value;
		return group.Id;
	}
}
```

The only valid methods are either static methods or instance methods of AgeTypes. Methods can be overloaded. Parameters can be passed by reference, arrays are always implicitly passed by reference. Local variables have the same restrictions as global variables except that non-array local variables can have initializers. There must be a single entrypoint method marked with ```[AgeMethod(EntryPoint = true)]``` which must return void and may not have any parameters.

Many C# code constructs are available such as ```if, else if, else``` conditions, ```for, while``` loops, ```break, continue``` branches, and assignments and method calls. Notable restrictions are:
+ You can only call other AgeMethods or the intrinsics defined in the ```Intrinsics``` class. The list of available intrinsics is constantly being expanded to include all necessary .per commands.
+ Nested accessors such as ```my_group_array[i].Id``` are not supported. You can either index an array variable like ```my_group_array[i]``` or a compound type variable like ```my_group.Id``` but not combined in one expression. You can go as deep as you want with field access though, such as ```my_manager.AttackGroup.Position.X```.
+ You can not define nor call any constructors, with the exception of array constructors which must be used to initialize array variables.
+ You can not assign one array to another if they do not have the same length. Technically arrays of different lengths are considered different types.
+ ```switch``` constructs are not supported. Use ```else if``` chains instead.
+ ```try catch``` constructs are not supported.
+ The only allowed ```throw``` statement is of the form ```throw new AgeException("my_custom_message")``` where ```my_custom_message``` must be a normal string literal. A throw statement will put a script into an 'exception state' where it will just chat the given message every subsequent tick and not do anything else anymore.
+ ```stackalloc``` is not supported nor any of the unsafe constructs.
+ And a bunch more, the parser should throw an appropriate exception if you try to do something it can't handle.

The following are currently still missing but are planned to be added in the future:
+ Conditional expressions.
+ Delegates or function pointers.
+ Generic methods.
+ Parameters with default values.
+ Global variables with initializers.
+ Defining ref local variables other than method parameters.
+ Operator overloading for custom compound types.

Take a look at the ```Deimos``` project for a full example of usage, the C# source code for the script is under the ```Source``` folder.