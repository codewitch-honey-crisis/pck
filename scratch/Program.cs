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
		//foreach (var test in _tests)
		//	Console.WriteLine(RegexExpression.Parse(test));
		_RunLalr(args);	
	}
	static void _RunLalr(string[] args)
	{
		var cfg = CfgDocument.ReadFrom(@"..\..\..\javascript.pck");
		var tokenizer = new JSTokenizer(new FileReaderEnumerable(@"..\..\..\hello.js"));
		var pt = cfg.ToLalr1ParseTable();
		var parser = new DebugLalr1Parser2(cfg, tokenizer, pt);
		parser.ShowHiddenTerminals =false;
		while (parser.Read())
		{
			Console.WriteLine("{0}: {1}, {2}", parser.NodeType, parser.Symbol, parser.Value);
		}
		parser = new DebugLalr1Parser2(cfg, tokenizer, pt);
		parser.ShowHiddenTerminals =true;
		while (LRNodeType.EndDocument != parser.NodeType)
			Console.WriteLine(parser.ParseReductions());

		return;
	}		
}

