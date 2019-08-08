﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pck
{
	partial class Program
	{
		static int _LL1Tree(string[] args)
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
		
			var tokenizer = lex.ToTokenizer(
				(1<args.Length)?(TextReaderEnumerable)new FileReaderEnumerable(args[1]):
					new ConsoleReaderEnumerable(),
				cfg.EnumSymbols());
			var parser = cfg.ToLL1Parser(tokenizer);
			while (LLNodeType.EndDocument != parser.NodeType)
				Console.WriteLine(parser.ParseSubtree());
			
			return 1;
		}
	}
}
