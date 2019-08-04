using Pck;
using System;
using System.Collections.Generic;
using System.IO;
using CharFA = Pck.CharFA<string>;
class Program
{
	static string[] _tests = new string[] {
	@"(""([^""\\]|\\.)*"")",		// 1
	@"\'([^\'\\]|\\.)*\'",		// 2
	@"([A-Z_a-z][\-0-9A-Z_a-z]*)",	// 3
	@"(\-?[0-9]+)",					// 4
	@"(( |(\v|(\f|(\t|(\r|\n))))))+",//5
	@"(//[^\n]*[\n])",				// 6
	@"/\*",							// 7
	@"\|",							// 8
	@"\}\+"							// 9
	};


	static void Main(string[] args)
	{
		//var exp = RegexExpression.Parse(_tests[4]);
		//var str = exp.ToString();
		//Console.WriteLine(str);
		//return;
		for(int i = 0;i<_tests.Length;++i)
		{
			Console.WriteLine("Testing {1} {0}", _tests[i],i+1);
			Console.WriteLine(RegexExpression.Parse(_tests[i]));
		}
		
		return;
	}		
}

