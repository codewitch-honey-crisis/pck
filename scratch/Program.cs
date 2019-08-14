﻿using Pck;
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
		_RunLL(args);
		_RunLalr(args);	
	}
	static void _RunLL(string[] args)
	{
		var cfg = CfgDocument.ReadFrom(@"..\..\..\expr.ll1.pck");
		var lex = LexDocument.ReadFrom(@"..\..\..\expr.ll1.pck");
		var tokenizer = lex.ToTokenizer("3+4*(2+1+1)"/*new FileReaderEnumerable(@"..\..\..\xbnf.xbnf")*/, cfg.FillSymbols());
		var parser = cfg.ToLL1Parser(tokenizer); //new LL1DebugParser(cfg,tokenizer);
		parser.ShowHidden = true;
		while (LLNodeType.EndDocument != parser.NodeType)
			Console.WriteLine(parser.ParseSubtree(true));

	}
	static void _RunLalr(string[] args)
	{
		var cfg = CfgDocument.ReadFrom(@"..\..\..\expr.pck");
		var lex = LexDocument.ReadFrom(@"..\..\..\expr.pck");
		var tokenizer =lex.ToTokenizer("3+4*(2+1+1)"/*new FileReaderEnumerable(@"..\..\..\data.json")*/, cfg.EnumSymbols());
		//var pt = cfg.ToLalr1ParseTable();// new _ConsoleProgress());
		var parser = cfg.ToLalr1Parser(tokenizer); //new Lalr1DebugParser(cfg, tokenizer, pt);
		
		/*parser.ShowHidden =false;
		while (parser.Read())
		{
			Console.WriteLine("{0}: {1}, {2} {3}", parser.NodeType, parser.Symbol, parser.Value,parser.Rule);
		}
		parser = new DebugLalr1Parser(cfg, tokenizer, pt);
		*/
		parser.ShowHidden =true;
		while (LRNodeType.EndDocument != parser.NodeType)
			Console.WriteLine(parser.ParseReductions(true));

		return;
	}		
	class _ConsoleProgress : IProgress<Lalr1Progress>
	{
		public void Report(Lalr1Progress progress)
		{
			switch(progress.Status)
			{
				case Lalr1Status.ComputingClosure:
				case Lalr1Status.ComputingMove:
				case Lalr1Status.ComputingConfigurations:
				case Lalr1Status.CreatingLookaheadGrammar:
					break;
				default:
					Console.Error.WriteLine("{0}: {1}", progress.Status, progress.Count);
					break;
			}
			
		}
	}
}

