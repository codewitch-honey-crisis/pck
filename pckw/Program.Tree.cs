using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pck
{
	partial class Program
	{
		static int _Tree(string[] args)
		{
			if (1> args.Length || args.Length>2)
			{
				_PrintUsage();
				return 1;
			}
			var pckspec = args[0];
			string pck;
			using (var sr = File.OpenText(pckspec))
				pck = sr.ReadToEnd();

			var cfg = CfgDocument.Parse(pck);
			var lex = LexDocument.Parse(pck);
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
			var tokenizer = new TableTokenizer(lexer.ToArray(syms), syms.ToArray(), bes, (args.Length>1)?(TextReaderEnumerable)new FileReaderEnumerable(args[1]):new ConsoleReaderEnumerable());
			var parser = cfg.ToParser(tokenizer);
			while (LL1ParserNodeType.EndDocument != parser.NodeType)
			{
				Console.WriteLine(parser.ParseSubtree());
			}
			return 1;
		}
	}
}
