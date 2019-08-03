using System;
using System.Collections.Generic;

namespace Pck
{
	class Program
	{
		static void Main(string[] args)
		{
			
			var text = "[a-z_A-Z][A-Za-z_0-9]*";
			Console.WriteLine(text);
			var ast = RegexExpression.Parse(text);
			var fa = ast.ToFA<string>();
			fa = ast.ToFA<string>().ToDfa();
			fa.TrimDuplicates();
			var closure = fa.FillClosure();
			Console.WriteLine(closure.Count);
			Console.WriteLine(closure[1].IsDuplicate(closure[2]));
			Console.WriteLine(ast);
			return;
		}
		
	}
}
