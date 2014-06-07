SharpSwift
==========
A C# to Swift language converter

###What is it?
SharpSwift is made up of three parts:

* The syntax converter converts C# code into Swift code, ready to be included in your iOS or OSX applications. The converter is located in the /SharpSwift/SharpSwift folder
* DNSwift provides implementations of basic .NET framework types in Swift. It does this by extending Swift types like String and Character with .NET methods like .Split() and .Trim(). DNSwift allows code converted with the syntax converter to call regular C# functions like "123".IndexOf('2') without worrying about how Swift implements the .IndexOf method. This is located in a separate repo, [matthewsot/DNSwift](https://github.com/matthewsot/DNSwift).
* Universals allow more complicated, language/platform-specific things like HttpClient and UI to work with the same C# code across multiple platforms. Universals consist of code written specifically for each language, so, for example, WebClient in C# will use HttpClient under-the-hood while WebClient in Swift will use NSURLConnection. Universals are located in another repo, [matthewsot/Universals](https://github.com/matthewsot/Universals)

###What isn't it?
This is not Xamarin. While it may eventually be able to replace or substitute Xamarin, it has quite a few differences. For one, it doesn't compile to native iOS/OSX code - it compiles into Swift code which can then be compiled into platform-specific code within XCode. You also can't do any kind of front-end design, at least without a reasonably advanced Universal designed for it.

It's also **not** ready for anything production - that's just crazy!

###What can it do?
SharpSwift currently can't do much except convert fairly simple C# syntax to Swift. As DNSwift and Universals get coded though, it will eventually allow you to use a single C# codebase across your iOS, OSX, Windows, and Windows Phone apps. The same three parts of SharpSwift could theoretically be done for Java as well, which would allow your one codebase to run against all major platforms.

The ultimate goal is to make a completely open-source alternative to Xamarin.

It's also a great project to help understand basic Swift syntax, as well as understand C# and the new Roslyn APIs a bit deeper, especially if you add to the converter source.

###What can't it do?
With that said, SharpSwift is currently nowhere near that goal. Right now, anything beyond simple classes and string manipulation won't work, and more advanced syntax features like Lambdas aren't supported.

###How can I contribute?
The best way you could contribute is to replace SharpSwift with another system based on the Mono project (like Xamarin currently is) :D

If you're not into that, though, there are definitely things you can do to help with SharpSwift.

If you'd like to implement conversion for a certain syntax simply add another method in the ``ConvertToSwift`` partial class located in SharpSwift/SharpSwift/Converters. As long as you add the ``ParsesTypeAttribute`` along with the Roslyn Syntax type of what you're converting it should work automatically.

For example, if you were going to add support for converting the syntax of methods you would use something like this:
```
[ParsesType(typeof (MethodDeclarationSyntax))]
public static string MethodDeclaration(MethodDeclarationSyntax node)
{
    var output = "func " + node.Identifier.Text + "(";    
    foreach (var parameter in node.ParameterList.Parameters)
    {
        output += parameter.Identifier.Text + ": " + Type(parameter.Type) + ", ";
    }
    
    output = output.Trim(' ', ',') + ") ";

    output += SyntaxNode(node.Body);
    return output;
}
```

The above code will be called to convert every ``MethodDeclarationSyntax`` node that comes up while parsing the C# document thanks to the ``ParsesTypeAttribute``. Note that we call ``Type(parameter.Type)`` to convert common C# types like ``string`` to their Swift conterparts (``String``), and that you should call ``SyntaxNode`` every time you think you're going to start parsing things that aren't strictly related to the ``MethodDeclarationSyntax`` (like a method's body, for example).


Also check out [DNSwift](https://github.com/matthewsot/DNSwift) and [Universals](https://github.com/matthewsot/Universals) if you'd like to help by writing Swift code.