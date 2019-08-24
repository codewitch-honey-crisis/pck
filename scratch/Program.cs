using Pck;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
		//_TestXbnfTokenizers(args);
		//foreach (var test in _tests)
		//	Console.WriteLine(RegexExpression.Parse(test));
		//_RunLL(args);
		//_RunLalr(args);	
		//_RunXbnfGenerated(args);
		//_RunDebugLalrXbnf(args);
		var xbnf = XbnfDocument.ReadFrom(@"..\..\..\xbnf.xbnf");
		var sb = new StringBuilder();
		var sw = new StringWriter(sb);
		XbnfToPckTransform.Transform(xbnf, sw);
		sw.Flush();

		//var cfg = CfgDocument.Parse(sb.ToString());
		//var lex = LexDocument.Parse(sb.ToString());
		//cfg.PrepareLL1();
		//var tokenizer = lex.ToTokenizer(new FileReaderEnumerable(@"..\..\..\xbnf.xbnf"), cfg.EnumSymbols());
		//var parser = cfg.ToLL1Parser(tokenizer); //new Lalr1DebugParser(cfg, tokenizer, pt);
		var parser = new XbnfParser(new XbnfTokenizer(new FileReaderEnumerable(@"..\..\..\xbnf.xbnf")));
			

		parser.ShowHidden = false;
		var pt = parser.ParseSubtree();
		var hasErrors = false;
		foreach (var pn in ParseNode.Select(pt.FillDescendantsAndSelf(),"#ERROR"))
		{
			hasErrors = true;
			Console.Error.WriteLine("Syntax Error: " + pn.Value);
		}
		if(!hasErrors)
		{
			var doc = new XbnfDocument();
			foreach(var pc in pt.Children)
			{
				doc.Productions.Add(_ParseProduction(pc));
			}
		}
	}
	static XbnfAttribute _ParseAttribute(ParseNode p)
	{
		var name = p.Children[0].Value;
		var v = ParseNode.SelectFirst(p.Children, XbnfParser.attrvalue);
		var val = (object)true;
		if (null!=v)
		{
			string s = v.Children[0].Value;
			val = ParseContext.Create(s).ParseJsonValue();
		}
		return new XbnfAttribute(name, val);
	}
	static XbnfExpression _ParseExpression(ParseNode p)
	{
		return null;
	}
	static XbnfProduction _ParseProduction(ParseNode p)
	{
		var result = new XbnfProduction();
		result.SetLocation(p.Line, p.Column, p.Position);
		result.Name = p.Children[0].Value;
		var apn = ParseNode.SelectFirst(p.Children, XbnfParser.attributes);
		if(null!=apn)
		{
			foreach(var ap in ParseNode.Select(apn.Children, XbnfParser.attribute)) 
				result.Attributes.Add(_ParseAttribute(ap));
		}
		var pexp = ParseNode.SelectFirst(p.Children, XbnfParser.orExpression);
		if (null!=pexp)
		{
			result.Expression=_ParseExpression(pexp);
		}
		Console.Error.WriteLine(result);
		return result;
	}
	static void _TestXbnfTokenizers(string[] args)
	{
		var cfg = CfgDocument.ReadFrom(@"..\..\..\xbnf.pck");
		var lex = LexDocument.ReadFrom(@"..\..\..\xbnf.pck");
		string input = null;
		using (var sr = File.OpenText(@"..\..\..\xbnf.xbnf"))
			input = sr.ReadToEnd();
		var tokenizer1 = lex.ToTokenizer(input, cfg.EnumSymbols());
		var tokenizer2 = tokenizer1;// new XbnfTokenizer(input);
		var t1 = new List<Token>(tokenizer1);
		var t2 = new List<Token>(tokenizer2);
		if (t1.Count != t2.Count)
			Console.Error.WriteLine("Counts are different.");
		for (int ic = t1.Count, i = 0; i < ic; ++i)
		{
			if (!Equals(t1[i], t2[i]))
			{
				Console.Error.WriteLine("at index {0}", i);
				Console.Error.WriteLine(t1[i]);
				Console.Error.WriteLine(t2[i]);
				break;
			}
		}

	}
	/*static void _RunXbnfGenerated(string[] args)
	{
		var cfg = CfgDocument.ReadFrom(@"..\..\..\xbnf.pck");
		var lex = LexDocument.ReadFrom(@"..\..\..\xbnf.pck");
		var tokenizer = new XbnfTokenizer(new FileReaderEnumerable(@"..\..\..\xbnf.xbnf"));
		var parser = new XbnfParser(tokenizer);
		parser.ShowHidden = true;
		while (LRNodeType.EndDocument != parser.NodeType)
			Console.WriteLine(parser.ParseReductions(true));

	}*/
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
	static void _RunLalrXbnf(string[] args)
	{
		var cfg = CfgDocument.ReadFrom(@"..\..\..\xbnf.pck");
		var lex = LexDocument.ReadFrom(@"..\..\..\xbnf.pck");
		var tokenizer = lex.ToTokenizer(new FileReaderEnumerable(@"..\..\..\xbnf.xbnf"), cfg.EnumSymbols());
		var parser = cfg.ToLalr1Parser(tokenizer); //new Lalr1DebugParser(cfg, tokenizer, pt);

		parser.ShowHidden = true;
		while (LRNodeType.EndDocument != parser.NodeType)
			Console.WriteLine(parser.ParseReductions(true));

	}
	static void _RunDebugLalrXbnf(string[] args)
	{
		var cfg = CfgDocument.ReadFrom(@"..\..\..\xbnf.pck");
		var lex = LexDocument.ReadFrom(@"..\..\..\xbnf.pck");
		IEnumerable<char> input = new FileReaderEnumerable(@"..\..\..\xbnf.xbnf");
		input = "foo<start>=bar;";
		var tokenizer = lex.ToTokenizer(input, cfg.EnumSymbols());
		var parser = cfg.ToLalr1Parser(tokenizer, new _ConsoleProgress());//new Lalr1DebugParser2(cfg, tokenizer);
		parser.ShowHidden = false;
		while (LRNodeType.EndDocument != parser.NodeType)
			Console.WriteLine(parser.ParseReductions());

	}
	static void _RunLalr(string[] args)
	{
		// we need both a lexer and a CfgDocument. 
		// we read them from the same file.
		var cfg = CfgDocument.ReadFrom(@"..\..\..\expr.pck");
		var lex = LexDocument.ReadFrom(@"..\..\..\expr.pck");
		// create a runtime tokenizer
		var tokenizer = lex.ToTokenizer("3*(4+7)", cfg.EnumSymbols());
		// create a parser
		var parser = cfg.ToLalr1Parser(tokenizer);

		/*parser.ShowHidden =false;
		while (parser.Read())
		{
			Console.WriteLine("{0}: {1}, {2} {3}", parser.NodeType, parser.Symbol, parser.Value,parser.Rule);
		}
		parser = new DebugLalr1Parser(cfg, tokenizer, pt);
		*/
		parser.ShowHidden = true;
		while (LRNodeType.EndDocument != parser.NodeType)
			Console.WriteLine(parser.ParseReductions(true));

		return;
	}

	class _ConsoleProgress : IProgress<CfgLalr1Progress>
	{
		public void Report(CfgLalr1Progress progress)
		{
			switch (progress.Status)
			{
				case CfgLalr1Status.ComputingClosure:
				case CfgLalr1Status.ComputingMove:
				case CfgLalr1Status.ComputingConfigurations:
				case CfgLalr1Status.CreatingLookaheadGrammar:
					break;
				default:
					Console.Error.WriteLine("{0}: {1}", progress.Status, progress.Count);
					break;
			}

		}

	}
}

