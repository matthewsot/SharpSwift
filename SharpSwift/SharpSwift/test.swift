//Converted with SharpSwift - https://github.com/matthewsot/SharpSwift//See https://github.com/matthewsot/DNSwift FMI about these includesinclude DNSwift;include DNSwift.System;include DNSwift.System.IO;include DNSwift.System.Text;include Something.Else;class test: ASCIIEncoding {	var somdething: String = "123";	func DoSomething<T>(del: (String, Int) -> String, input: T) -> T {		return input;	}	enum Something: Int {		Some = 1,		Another,		Third = 3	}	init(something: String) {		var ints = [ 0, 1, 2 ];		var y = test("hello");		var x: String, z: String = "123";		var reader = StreamReader("");		var d = "1232";		reader = nil;	}}