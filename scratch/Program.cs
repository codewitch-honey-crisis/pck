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
		var cfg = CfgDocument.ReadFrom(@"..\..\..\xbnf.pck");
		var tokenizer = new XbnfTokenizer(new FileReaderEnumerable(@"..\..\..\xbnf.xbnf"));
		var pt = cfg.ToLalrParseTable();
		var parser = new DebugLalrParser(cfg, tokenizer, pt);
		while(LRNodeType.EndDocument!=parser.NodeType)
		{
			Console.WriteLine(parser.ParseReductions());
		}

		return;
	}		
}

