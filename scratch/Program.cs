using Pck;
using System;
using System.Collections.Generic;
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
		//	Console.WriteLine("Testing {1} {0}", _tests[i],i+1);
			//Console.WriteLine(RegexExpression.Parse(_tests[i]));
		}

		var pckspec = @"..\..\..\xbnf.ll1.pck";
		var input = @"..\..\..\xbnf.xbnf";
;
		var cfg = CfgDocument.ReadFrom(pckspec);
		var lex = LexDocument.ReadFrom(pckspec);
		var lexer = lex.ToLexer();
		var ii = 0;
		var syms = new List<string>();
		cfg.FillSymbols(syms);
		var bes = new string[syms.Count];
		for (ii = 0; ii < bes.Length; ii++)
			bes[ii] = lex.GetAttribute(syms[ii], "blockEnd", null) as string;
		var dfaTable = lexer.ToArray(syms);
		var tt = new List<string>();
		for (int ic = lex.Rules.Count, i = 0; i < ic; ++i)
		{
			var t = lex.Rules[i].Left;
			if (!tt.Contains(t))
				tt.Add(t);
		}
		tt.Add("#EOS");
		tt.Add("#ERROR");
		for (int ic = syms.Count, i = 0; i < ic; ++i)
		{
			if (!tt.Contains(syms[i]))
				syms[i] = null;
		}
		var tokenizer = new TableTokenizer(lexer.ToArray(syms), syms.ToArray(), bes, new FileReaderEnumerable(input));
		var parser = cfg.ToParser(tokenizer);
		foreach(var tok in tokenizer)
		{
			Console.WriteLine("{0}: {1}", tok.Symbol, tok.Value);
		}
		return;
	}		
}

