using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace Pck
{
	[Transform("pckToLex",".pck",".l","Translates a pck spec to a lex/flex spec")]
	class PckToLexTransform
	{
		public static void Transform(TextReader input, TextWriter output)
			=> Transform(LexDocument.ReadFrom(input), output);
		public static void Transform(LexDocument lex,TextWriter output)
		{
			output.WriteLine("%%");
			for(int ic=lex.Rules.Count,i=0;i<ic;++i)
			{
				var rule = lex.Rules[i];
				var o = lex.GetAttribute(rule.Left, "hidden", false);
				if (o is bool && (bool)o)
				{
					var s = string.Concat(rule.Right, "\t;");
					var be = lex.GetAttribute(rule.Left, "blockEnd", null) as string;
					if (!string.IsNullOrEmpty(be))
						s = string.Concat(s, " /* TODO: implement blockend */");
					output.WriteLine(s);
				}
				else
				{
					var s = string.Concat(rule.Right, "\treturn ", string.Concat(_EscId(rule.Left), ";"));
					var be = lex.GetAttribute(rule.Left, "blockEnd", null) as string;
					if (!string.IsNullOrEmpty(be))
						s = string.Concat(s, " /* TODO: implement blockend */");
					output.WriteLine(s);
				}
			}
		}
		static string _EscId(string id)
		{
			return id;
		}
	}
}
